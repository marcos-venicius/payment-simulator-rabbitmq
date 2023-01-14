using System.Text;
using Core.RabbitMq.Models;
using Core.Services.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Core.Services;

public sealed class RabbitMqPaymentProducerService : IMessageProducerService<PaymentData>
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    private const string ExchangeName = "payments-exchange";
    private const string QueueName = "payments-queue";
    private const string RouteName = "make-payment";

    public RabbitMqPaymentProducerService()
    {
        var factory = new ConnectionFactory
        {
            UserName = "user",
            Password = "password",
            HostName = "localhost"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: ExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            arguments: null
        );

        _channel.QueueDeclare(
            queue: QueueName,
            durable: true,
            autoDelete: false,
            exclusive: false,
            arguments: null
        );

        _channel.QueueBind(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: RouteName,
            arguments: null
        );
    }

    public void Publish(PaymentData data)
    {
        var properties = _channel.CreateBasicProperties();

        properties.Persistent = true;

        var serialized = JsonConvert.SerializeObject(data);

        var body = Encoding.UTF8.GetBytes(serialized);
        
        _channel.BasicPublish(
            exchange: ExchangeName,
            routingKey: RouteName,
            basicProperties: properties,
            body: body
        );
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}