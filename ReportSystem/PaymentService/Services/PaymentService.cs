using Commons.Exceptions;
using PaymentService.RabbitMq.Models;
using PaymentService.Repositories.Interfaces;
using PaymentService.Services.Interfaces;

namespace PaymentService.Services;

public class PaymentService : IPaymentService
{
    private readonly ILogger<PaymentService> _logger;
    private readonly IPaymentRepository _paymentRepository;

    public PaymentService(IPaymentRepository paymentRepository, ILogger<PaymentService> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task Pay(PaymentData data)
    {
        var payment = await _paymentRepository.FindByIdAsync(data.Id);

        if (payment is null)
        {
            _logger.LogError("[{}] NOT EXISTS", data.Id);
            throw new RejectException("este pagamento não existe");
        }

        try
        {
            if (payment.State != "waiting")
            {
                await _paymentRepository.UpdateStatusAsync(data.Id, "retrying");
                _logger.LogInformation("[{}] RETRYING PAYMENT", data.Id);
            }
            else
            {
                await _paymentRepository.SaveAsync(data);
                _logger.LogInformation("[{}] NEW PAYMENT", data.Id);
            }

            await Task.Delay(5000);

            var random = new Random();

            // simulate fail
            if (random.Next(0, 10000) % 2 != 0 && random.Next(0, 11) % 2 != 0)
                throw new Exception();

            _logger.LogInformation("[{}] PAID", data.Id);

            await _paymentRepository.UpdateStatusAsync(data.Id, "success");
        }
        catch
        {
            _logger.LogError("[{}] FAIL", data.Id);

            await _paymentRepository.UpdateStatusAsync(data.Id, "fail");

            throw;
        }
    }
}