using Abp.Domain.Repositories;
using Abp.Domain.Services;
using DMS.Invoices;
using DMS.Orders;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Customers;

public class CreditCheckService : DomainService
{
    private readonly IRepository<Customer, int> _customerRepository;
    private readonly IRepository<Invoice, int> _invoiceRepository;
    private readonly IRepository<Order, int> _orderRepository;

    public CreditCheckService(
        IRepository<Customer, int> customerRepository,
        IRepository<Invoice, int> invoiceRepository,
        IRepository<Order, int> orderRepository)
    {
        _customerRepository = customerRepository;
        _invoiceRepository = invoiceRepository;
        _orderRepository = orderRepository;
    }

    public async Task<CreditCheckResult> CheckCreditAsync(int customerId, decimal orderTotal)
    {
        var customer = await _customerRepository.GetAsync(customerId);

        if (!customer.CreditEnabled)
        {
            return new CreditCheckResult
            {
                IsOverLimit = false,
                CreditLimit = customer.CreditLimit,
                OutstandingBalance = 0m,
                AvailableCredit = customer.CreditLimit
            };
        }

        // Outstanding balance = sum of unpaid invoice amounts for this customer
        // Invoice links to Order which has CustomerId
        var customerOrderIds = _orderRepository.GetAll()
            .Where(o => o.CustomerId == customerId)
            .Select(o => o.Id);

        var outstandingBalance = await _invoiceRepository.GetAll()
            .Where(i => customerOrderIds.Contains(i.OrderId) &&
                        i.Status != InvoiceStatus.Voided)
            .SumAsync(i => i.Total - i.PaidAmount);

        var availableCredit = customer.CreditLimit - outstandingBalance;

        return new CreditCheckResult
        {
            IsOverLimit = (outstandingBalance + orderTotal) > customer.CreditLimit,
            CreditLimit = customer.CreditLimit,
            OutstandingBalance = outstandingBalance,
            AvailableCredit = availableCredit
        };
    }
}
