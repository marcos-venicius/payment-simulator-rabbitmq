using PaymentService.Delegates;

namespace PaymentService.Services.Interfaces;

public interface IMessageConsumerService<out T> : IDisposable where T : class
{
    event ConsumerEvent<T> OnReceived;
    
    void Listen();
}