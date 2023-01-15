using Core.Models;
using Core.RabbitMq.Models;

namespace Core.Repositories.Interfaces;

public interface IPaymentRepository
{
    Task<IEnumerable<Payment>> ListAsync();
    Task SaveAsync(PaymentData data);
}