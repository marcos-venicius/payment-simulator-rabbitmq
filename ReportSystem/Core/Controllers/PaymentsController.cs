using Core.Models;
using Core.Services.Interfaces;
using Core.Services.Requests.PaymentServiceRequests;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers;

[ApiController]
[Route("/api/v1/payments")]
public sealed class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Payment>>> List()
    {
        var data = await _paymentService.ListAsync();

        return Ok(data);
    }

    [HttpPost]
    public IActionResult Create(CreatePaymentRequest request)
    {
        _paymentService.CreatePayment(request);
        
        return new JsonResult(new
        {
            Message = "Seu pagamento está sendo processado"
        });
    }
}