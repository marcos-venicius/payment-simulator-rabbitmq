using Core.Models;

namespace Core.Repositories.Interfaces;

public interface IPaymentRepository
{
    Task<IEnumerable<Payment>> ListAsync();
}