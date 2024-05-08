using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MrPill.DTOs.DTOs;
using System.Text.Json;
using UserServiceApp.Models.UserService;

public class RabbitMQHostedService : IHostedService
{
    private IConnection? cnn;
    private IModel? channel;
    private readonly IUserService _userService;
    private readonly ILogger<IUserService> _logger;

    public RabbitMQHostedService(IUserService userService, ILogger<IUserService> logger) 
    {
        _userService = userService;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        ConnectionFactory factory = new()
        {
            Uri = new Uri(RabbitMqConstants.Uri),
            ClientProvidedName = RabbitMqConstants.GerResponseFromServerQueueName
        };

        cnn = factory.CreateConnection();
        channel = cnn.CreateModel();

        channel.ExchangeDeclare(RabbitMqConstants.ExchangeName, ExchangeType.Direct);
        channel.QueueDeclare(RabbitMqConstants.QueueName, false, false,false, null);
        channel.QueueBind(RabbitMqConstants.QueueName, RabbitMqConstants.ExchangeName, RabbitMqConstants.RoutingKey, null);
        channel.BasicQos(0,1,false); // prefetchSize, prefetchCount, global 

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (sender, args) =>
        {
            var body = args.Body.ToArray();
            string json = Encoding.UTF8.GetString(body);
          
            LoginComunicationDWrapper wrapper = JsonSerializer.Deserialize<LoginComunicationDWrapper>(json)!;

            if (wrapper != null)
            {
                _userService.SaveMassageToManagerHouseToAddNewUser(wrapper);
                channel.BasicAck(args.DeliveryTag, false);
            }
            else
            {
                _logger.LogWarning("Failed to deserialize JSON into LoginComunicationDWrapper object");
            } 
        };

        string consumeTag = channel.BasicConsume(RabbitMqConstants.QueueName, false, consumer);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        channel?.Close();
        cnn?.Close();
        return Task.CompletedTask;
    }
}
