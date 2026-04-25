using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using DMS.Authorization;
using DMS.Common.Dto;
using DMS.Invoices;
using DMS.Payments.Dto;
using DMS.Payments.Pdf;
using DMS.Payments.Storage;
using Microsoft.EntityFrameworkCore;

namespace DMS.Payments;

[AbpAuthorize(PermissionNames.Pages_Payments)]
public class PaymentAppService : DMSAppServiceBase, IPaymentAppService
{
    private readonly IRepository<Payment, int> _paymentRepository;
    private readonly IRepository<PaymentLine, int> _lineRepository;
    private readonly IRepository<PaymentMethod, int> _methodRepository;
    private readonly IRepository<Invoice, int> _invoiceRepository;
    private readonly ReceiptNumberGenerator _receiptNumberGenerator;
    private readonly IReceiptPdfService _pdfService;
    private readonly IReceiptFileStore _fileStore;

    public PaymentAppService(
        IRepository<Payment, int> paymentRepository,
        IRepository<PaymentLine, int> lineRepository,
        IRepository<PaymentMethod, int> methodRepository,
        IRepository<Invoice, int> invoiceRepository,
        ReceiptNumberGenerator receiptNumberGenerator,
        IReceiptPdfService pdfService,
        IReceiptFileStore fileStore)
    {
        _paymentRepository = paymentRepository;
        _lineRepository = lineRepository;
        _methodRepository = methodRepository;
        _invoiceRepository = invoiceRepository;
        _receiptNumberGenerator = receiptNumberGenerator;
        _pdfService = pdfService;
        _fileStore = fileStore;
    }

    [AbpAuthorize(PermissionNames.Pages_Payments_Create)]
    public async Task<ApiResponse<PaymentDto>> RecordPaymentAsync(RecordPaymentDto input)
    {
        var invoice = await _invoiceRepository.GetAll()
            .FirstOrDefaultAsync(i => i.Id == input.InvoiceId)
            ?? throw new UserFriendlyException("Invoice not found.");

        if (invoice.Status != InvoiceStatus.Issued && invoice.Status != InvoiceStatus.PartiallyPaid)
            throw new UserFriendlyException("Invoice is not in a payable state.");

        var total = input.Lines.Sum(l => l.Amount);
        var remaining = invoice.Total - invoice.PaidAmount;

        if (total > remaining)
            throw new UserFriendlyException("Payment amount exceeds invoice balance.");

        var receiptNumber = await _receiptNumberGenerator.GenerateAsync(AbpSession.TenantId!.Value);

        var payment = new Payment
        {
            TenantId = AbpSession.TenantId!.Value,
            InvoiceId = invoice.Id,
            ReceiptNumber = receiptNumber,
            PaymentDate = input.PaymentDate,
            TotalAmount = total,
            Notes = input.Notes
        };

        await _paymentRepository.InsertAsync(payment);
        await CurrentUnitOfWork.SaveChangesAsync();

        foreach (var lineDto in input.Lines)
        {
            await _lineRepository.InsertAsync(new PaymentLine
            {
                TenantId = payment.TenantId,
                PaymentId = payment.Id,
                PaymentMethodId = lineDto.PaymentMethodId,
                Amount = lineDto.Amount,
                Reference = lineDto.Reference
            });
        }

        invoice.PaidAmount += total;
        invoice.Status = invoice.PaidAmount >= invoice.Total ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid;
        await _invoiceRepository.UpdateAsync(invoice);
        await CurrentUnitOfWork.SaveChangesAsync();

        var pdfModel = await BuildReceiptPdfModelAsync(payment, invoice);
        var pdfBytes = _pdfService.Generate(pdfModel);
        await _fileStore.SaveAsync(payment.TenantId, receiptNumber, pdfBytes);
        payment.ReceiptFilePath = $"{payment.TenantId}/{receiptNumber}.pdf";
        await _paymentRepository.UpdateAsync(payment);
        await CurrentUnitOfWork.SaveChangesAsync();

        return await GetAsync(payment.Id);
    }

    public async Task<ApiResponse<PaymentDto>> GetAsync(int id)
    {
        var payment = await _paymentRepository.GetAll()
            .Include(p => p.Lines)
                .ThenInclude(l => l.PaymentMethod)
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new UserFriendlyException("Payment not found.");

        return Ok(ObjectMapper.Map<PaymentDto>(payment), L("RetrievedSuccessfully"));
    }

    public async Task<ApiResponse<PagedResultDto<PaymentDto>>> GetAllAsync(PagedPaymentRequestDto input)
    {
        var query = _paymentRepository.GetAll()
            .Include(p => p.Lines)
                .ThenInclude(l => l.PaymentMethod)
            .Where(p => p.TenantId == AbpSession.TenantId);

        if (input.InvoiceId.HasValue)
            query = query.Where(p => p.InvoiceId == input.InvoiceId.Value);

        if (input.FromDate.HasValue)
            query = query.Where(p => p.PaymentDate >= input.FromDate.Value);

        if (input.ToDate.HasValue)
            query = query.Where(p => p.PaymentDate <= input.ToDate.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.PaymentDate)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToListAsync();

        return Ok(new PagedResultDto<PaymentDto>(total,
            ObjectMapper.Map<System.Collections.Generic.List<PaymentDto>>(items)), L("RetrievedSuccessfully"));
    }

    [AbpAuthorize(PermissionNames.Pages_Payments_GetReceipt)]
    public async Task<ApiResponse<byte[]>> GetReceiptBytesAsync(int paymentId)
    {
        var payment = await _paymentRepository.GetAll()
            .Include(p => p.Lines).ThenInclude(l => l.PaymentMethod)
            .FirstOrDefaultAsync(p => p.Id == paymentId)
            ?? throw new UserFriendlyException("Payment not found.");

        var stored = await _fileStore.LoadAsync(payment.TenantId, payment.ReceiptNumber);
        if (stored != null)
            return Ok(stored, L("RetrievedSuccessfully"));

        return Ok(await RegenerateAndSaveAsync(payment), L("RetrievedSuccessfully"));
    }

    [AbpAuthorize(PermissionNames.Pages_Payments_RegenerateReceipt)]
    public async Task<ApiResponse<byte[]>> RegenerateReceiptAsync(int paymentId)
    {
        var payment = await _paymentRepository.GetAll()
            .Include(p => p.Lines).ThenInclude(l => l.PaymentMethod)
            .FirstOrDefaultAsync(p => p.Id == paymentId)
            ?? throw new UserFriendlyException("Payment not found.");

        return Ok(await RegenerateAndSaveAsync(payment), L("RetrievedSuccessfully"));
    }

    private async Task<byte[]> RegenerateAndSaveAsync(Payment payment)
    {
        var invoice = await _invoiceRepository.GetAll()
            .FirstOrDefaultAsync(i => i.Id == payment.InvoiceId)
            ?? throw new UserFriendlyException("Invoice not found.");

        var pdfModel = await BuildReceiptPdfModelAsync(payment, invoice);
        var bytes = _pdfService.Generate(pdfModel);
        await _fileStore.SaveAsync(payment.TenantId, payment.ReceiptNumber, bytes);
        payment.ReceiptFilePath = $"{payment.TenantId}/{payment.ReceiptNumber}.pdf";
        await _paymentRepository.UpdateAsync(payment);
        return bytes;
    }

    private async Task<ReceiptPdfModel> BuildReceiptPdfModelAsync(Payment payment, Invoice invoice)
    {
        var methodIds = payment.Lines.Select(l => l.PaymentMethodId).ToList();
        var methods = await _methodRepository.GetAll()
            .Where(m => methodIds.Contains(m.Id))
            .ToListAsync();

        var paidBefore = invoice.PaidAmount - payment.TotalAmount;

        return new ReceiptPdfModel
        {
            ReceiptNumber = payment.ReceiptNumber,
            PaymentDate = payment.PaymentDate.ToString("dd MMM yyyy"),
            CustomerName = invoice.CustomerName,
            CustomerAddress = invoice.CustomerAddress ?? string.Empty,
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceTotal = invoice.Total,
            PaidBeforeThis = paidBefore < 0 ? 0 : paidBefore,
            TotalPaid = payment.TotalAmount,
            RemainingBalance = invoice.Total - invoice.PaidAmount,
            InvoiceStatus = invoice.Status == InvoiceStatus.Paid ? "PAID" : "PARTIALLY PAID",
            CompanyName = "Company Name",
            CompanyAddress = string.Empty,
            Lines = payment.Lines.Select(l => new ReceiptLineModel
            {
                MethodName = methods.FirstOrDefault(m => m.Id == l.PaymentMethodId)?.Name ?? string.Empty,
                Reference = l.Reference,
                Amount = l.Amount
            }).ToList()
        };
    }
}
