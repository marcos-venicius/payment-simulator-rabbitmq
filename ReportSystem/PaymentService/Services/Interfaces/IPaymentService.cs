using PaymentService.RabbitMq.Models;

namespace PaymentService.Services.Interfaces;

public interface IPaymentService
{
    public Task Pay(PaymentData data);
}