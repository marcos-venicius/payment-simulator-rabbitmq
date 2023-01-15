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

    private const string QueueName = "payments";
    private const string ExchangeName = "payments";
    
    private const string QueueRetryName = "payments-retry";
    private const string ExchangeRetryName = "payments-retry";

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
            type: ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            arguments: null
        );

        _channel.ExchangeDeclare(
            exchange: ExchangeRetryName,
            type: ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            arguments: null
        );

        _channel.QueueDeclare(
            queue: QueueName,
            durable: true,
            autoDelete: false,
            exclusive: false,
            arguments: new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", ExchangeRetryName }
            }
        );

        _channel.QueueDeclare(
            queue: QueueRetryName,
            durable: true,
            autoDelete: false,
            exclusive: false,
            arguments: new Dictionary<string, object>
            {
                { "x-message-ttl", 10_000 },
                { "x-dead-letter-exchange", ExchangeName }
            }
        );

        _channel.QueueBind(
            exchange: QueueName,
            queue: ExchangeName,
            routingKey: "",
            arguments: null
        );

        _channel.QueueBind(
            exchange: ExchangeRetryName,
            queue: QueueRetryName,
            routingKey: "",
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
            exchange: QueueName,
            routingKey: "make-payment",
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