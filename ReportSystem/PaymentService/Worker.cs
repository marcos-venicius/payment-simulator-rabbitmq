using PaymentService.RabbitMq.Models;
using PaymentService.Services.Interfaces;

namespace PaymentService;

public class Worker : BackgroundService
{
    private readonly IPaymentService _paymentService;
    private readonly IMessageConsumerService<PaymentData> _messageConsumerPaymentService;

    public Worker(IMessageConsumerService<PaymentData> messageConsumerPaymentService, IPaymentService paymentService)
    {
        _messageConsumerPaymentService = messageConsumerPaymentService;
        _paymentService = paymentService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _messageConsumerPaymentService.OnReceived += _paymentService.Pay;

        _messageConsumerPaymentService.Listen();

        return Task.CompletedTask;
    }
}