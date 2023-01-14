using PaymentService.RabbitMq.Models;
using PaymentService.Repositories.Interfaces;
using PaymentService.Services.Interfaces;

namespace PaymentService;

public class Worker : BackgroundService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMessageConsumerService<PaymentData> _messageConsumerPaymentService;

    public Worker(
        IMessageConsumerService<PaymentData> messageConsumerPaymentService,
        IPaymentRepository paymentRepository)
    {
        _messageConsumerPaymentService = messageConsumerPaymentService;
        _paymentRepository = paymentRepository;
    }

    private async Task ProcessPayment(PaymentData data)
    {
        var savedOnDatabase = await _paymentRepository.ExistsAsync(data.Id);
        
        try
        {
            Console.WriteLine(savedOnDatabase
                ? $"[{data.Id}] RETRY PAYMENT \t{data.Name}\t"
                : $"[{data.Id}] NEW PAYMENT \t{data.Name}\t"
            );

            if (!savedOnDatabase)
                await _paymentRepository.SaveAsync(data);
            else
                await _paymentRepository.UpdateStatusAsync(data.Id, "retrying");

            savedOnDatabase = true;
            
            Console.WriteLine($"[{data.Id}] SAVE LOG \t{data.Name}\t");

            await Task.Delay(5000);

            var random = new Random();

            // simulate fail
            if (random.Next(0, 10000) % 2 != 0 && random.Next(0, 11) % 2 != 0)
                throw new Exception();
            
            Console.WriteLine($"[{data.Id}] PAID \t\t{data.Name}\t");
            
            await _paymentRepository.UpdateStatusAsync(data.Id, "success");
        }
        catch
        {
            Console.WriteLine($"[{data.Id}] FAIL \t\t{data.Name}\t");
            
            if (!savedOnDatabase)
                await _paymentRepository.SaveAsync(data);
            
            await _paymentRepository.UpdateStatusAsync(data.Id, "fail");
            
            throw;
        }
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _messageConsumerPaymentService.OnReceived += ProcessPayment;
        
        _messageConsumerPaymentService.Listen();
        
        return Task.CompletedTask;
    }
}