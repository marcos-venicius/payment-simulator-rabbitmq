using PaymentService.RabbitMq.Models;

namespace PaymentService.Repositories.Interfaces;

public interface IPaymentRepository
{
    Task<bool> ExistsAsync(Guid id);
    Task SaveAsync(PaymentData data);
    Task UpdateStatusAsync(Guid id, string status);
}