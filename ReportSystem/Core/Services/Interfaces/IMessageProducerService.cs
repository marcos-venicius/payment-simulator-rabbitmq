namespace Core.Services.Interfaces;

public interface IMessageProducerService<in T> : IDisposable where T : class
{
    void Publish(T data);
}