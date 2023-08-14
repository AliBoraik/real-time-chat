using System.Text.Json;
using Chat.Domain.Dto;
using Chat.Domain.Messages;
using Chat.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Chat.Api.Consumer;

public class RabbitMqProducer : BackgroundService
{
    private readonly IMongoDbContext _mongoDb;
    private readonly string _queueName;
    private readonly ICacheService _cacheService;
    private IModel _channel;
    private IConnection _connection;
    private ConnectionFactory _connectionFactory;

    public RabbitMqProducer(IMessageService messageService, ICacheService cacheService, IMongoDbContext mongoDb)
    {
        _cacheService = cacheService;
        _mongoDb = mongoDb;
        _queueName = "ChatApp.DataUploaded";
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
                var message = JsonSerializer.Deserialize<DataUploadedMessage>(body);

                //_cacheService.ChangeDatabase(Database.Meta);
                var metaJson = _cacheService.GetData(message.RequestId.ToString());
                var meta = JsonSerializer.Deserialize<MongoFile>(metaJson);
                await _mongoDb.CreateAsync(meta);

                //_cacheService.ChangeDatabase(Database.File);
                var file = _cacheService.GetData(message.RequestId.ToString());
                //todo: move to persist bucket
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