using Core.Models;
using Core.RabbitMq.Models;
using Core.Repositories.Interfaces;
using Core.Services.Interfaces;
using Core.Services.Requests.PaymentServiceRequests;

namespace Core.Services;

public sealed class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMessageProducerService<PaymentData> _paymentMessageProducerService;

    public PaymentService(IMessageProducerService<PaymentData> paymentMessageProducerService, IPaymentRepository paymentRepository)
    {
        _paymentMessageProducerService = paymentMessageProducerService;
        _paymentRepository = paymentRepository;
    }

    public async Task CreatePayment(CreatePaymentRequest request)
    {
        var paymentData = new PaymentData(Guid.NewGuid(), request.Name, request.Price);

        await _paymentRepository.SaveAsync(paymentData);
        
        _paymentMessageProducerService.Publish(paymentData);
    }

    public async Task<List<Payment>> ListAsync()
    {
        var list = await _paymentRepository.ListAsync();

        return list.ToList();
    }
}