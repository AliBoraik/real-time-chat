using System.Text;
using System.Text.Json;
using Chat.Domain.Messages;
using RabbitMQ.Client;

namespace Chat.BackgroundService;

public class Producer
{
    private readonly string _queueName;

    public Producer()
    {
        _queueName = "ChatApp.DataUploaded";
    }

    public void SendMessage(DataUploadedMessage message)
    {
        var factory = new ConnectionFactory { HostName = "rabbitmq" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(_queueName,
                false,
                false,
                false,
                null);

            var messageJson = JsonSerializer.Serialize(message);

            var body = Encoding.UTF8.GetBytes(messageJson);

            channel.BasicPublish("",
                _queueName,
                null,
                body);
        }
    }
}