using System.Text.Json;
using Chat.Domain.Entities;
using Chat.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Chat.BackgroundService.Handlers;

public class MessageSentHandler : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly string _queueName;
    private IModel _channel;
    private IConnection _connection;
    private ConnectionFactory _connectionFactory;
    private readonly IMessageService _messageService;

    public MessageSentHandler(IMessageService messageService)
    {
        _messageService = messageService;
        _queueName = "ChatApp.Messages";
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
                var message = JsonSerializer.Deserialize<Message>(body);
                await _messageService.Create(message);
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