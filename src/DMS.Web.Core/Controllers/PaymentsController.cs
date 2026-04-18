using System.Threading.Tasks;
using DMS.Payments;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Controllers
{
    [Route("api/payments")]
    public class PaymentsController : DMSControllerBase
    {
        private readonly IPaymentAppService _paymentService;

        public PaymentsController(IPaymentAppService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("{id}/receipt")]
        public async Task<IActionResult> GetReceipt(int id)
        {
            var bytes = await _paymentService.GetReceiptBytesAsync(id);
            return File(bytes, "application/pdf", $"receipt-{id}.pdf");
        }

        [HttpPost("{id}/receipt/regenerate")]
        public async Task<IActionResult> RegenerateReceipt(int id)
        {
            var bytes = await _paymentService.RegenerateReceiptAsync(id);
            return File(bytes, "application/pdf", $"receipt-{id}.pdf");
        }
    }
}
