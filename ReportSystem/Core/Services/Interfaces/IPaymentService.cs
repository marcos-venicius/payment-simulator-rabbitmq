using Core.Models;
using Core.Services.Requests.PaymentServiceRequests;

namespace Core.Services.Interfaces;

public interface IPaymentService
{
    Task CreatePayment(CreatePaymentRequest request);
    Task<List<Payment>> ListAsync();
}