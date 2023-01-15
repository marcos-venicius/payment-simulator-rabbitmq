using System.Text;
using Newtonsoft.Json;
using PaymentService.Delegates;
using PaymentService.RabbitMq.Models;
using PaymentService.Services.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PaymentService.Services;

public sealed class RabbitMqPaymentConsumerService : IMessageConsumerService<PaymentData>
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    private const string QueueName = "payments";
    private const string ExchangeName = "payments";
    
    private const string QueueRetryName = "payments-retry";
    private const string ExchangeRetryName = "payments-retry";

    public RabbitMqPaymentConsumerService()
    {
        var factory = new ConnectionFactory
        {
            UserName = "user",
            Password = "password",
            HostName = "localhost",
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

        _channel.BasicQos(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false
        );
    }

    public event ConsumerEvent<PaymentData>? OnReceived;

    public void Listen()
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (_, ea) =>
        {
            try
            {
                var serialized = Encoding.UTF8.GetString(ea.Body.ToArray());

                var deserializeObject = JsonConvert.DeserializeObject<PaymentData>(serialized);

                if (deserializeObject is null)
                    throw new ApplicationException("CANNOT_DESERIALIZED_OBJECT");

                if (OnReceived is not null)
                    await OnReceived.Invoke(deserializeObject);

                _channel.BasicAck(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false
                );
            }
            catch
            {
                _channel.BasicNack(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    requeue: false
                );
            }
        };

        _channel.BasicConsume(
            queue: QueueName,
            autoAck: false,
            consumer: consumer
        );
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}