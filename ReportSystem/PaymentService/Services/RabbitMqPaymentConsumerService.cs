using System.Text;
using Commons.Exceptions;
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

    private const string QueueName = "payments-queue";

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

        _channel.QueueDeclare(
            queue: QueueName,
            durable: true,
            autoDelete: false,
            exclusive: false,
            arguments: null
        );

        _channel.BasicQos(
            prefetchSize: 0,
            prefetchCount: 3,
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
            catch (RejectException)
            {
                _channel.BasicReject(
                    deliveryTag: ea.DeliveryTag,
                    requeue: false
                );
            }
            catch
            {
                _channel.BasicNack(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    requeue: true
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