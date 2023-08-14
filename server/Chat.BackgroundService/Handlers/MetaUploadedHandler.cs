using System.Text.Json;
using Chat.Domain.Messages;
using Chat.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Enums;

namespace Chat.BackgroundService.Handlers;

public class MetaUploadedHandler : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly Producer _producer;
    private readonly string _queueName;
    private readonly ICacheService _cacheService;
    private IModel _channel;
    private IConnection _connection;
    private ConnectionFactory _connectionFactory;

    public MetaUploadedHandler(ICacheService cacheService, Producer producer)
    {
        _cacheService = cacheService;
        _producer = producer;
        _cacheService.ChangeDatabase(Database.Common);
        _queueName = "ChatApp.Meta";
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _connectionFactory = new ConnectionFactory
        {
            HostName = "rabbitmq"
        };

        _connection = _connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(_queueName,
            false,
            false,
            false,
            null);
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<MetaUploadMessage>(body);

                _cacheService.Increment(message.RequestId.ToString());
                var counter = _cacheService.GetData(message.RequestId.ToString());

                if (counter == "2") _producer.SendMessage(new DataUploadedMessage { RequestId = message.RequestId });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        };

        _channel.BasicConsume(_queueName, true, consumer);

        await Task.CompletedTask;
    }
}