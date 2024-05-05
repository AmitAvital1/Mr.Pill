using RabbitMQ.Client;
using MrPill.DTOs.DTOs;

namespace Login.Models.RabbitMq;
public class RabbitMqHandler : IHostedService
{
    private static RabbitMqHandler? instance;
    private static readonly object lockObject = new();

    private RabbitMqHandler() { }

    public static RabbitMqHandler Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    instance ??= new RabbitMqHandler();
                }
            }
            
            return instance;
        }
    }

    public void SendMassage(LoginComunicationDWrapper loginComunicationDWrapper)
    {
        ConnectionFactory factory = new();
        factory.Uri = new Uri(RabbitMqConstants.Uri);
        factory.ClientProvidedName = RabbitMqConstants.ClientProvidedName;

        IConnection cnn = factory.CreateConnection();

        IModel channel = cnn.CreateModel();

        channel.ExchangeDeclare(RabbitMqConstants.ExchangeName, ExchangeType.Direct);
        channel.QueueDeclare(RabbitMqConstants.QueueName, false, false,false, null);
        channel.QueueBind(RabbitMqConstants.QueueName, RabbitMqConstants.ExchangeName, RabbitMqConstants.RoutingKey, null);
        
        byte[] messageBodyBytes = loginComunicationDWrapper.GetBytes();
        channel.BasicPublish(RabbitMqConstants.ExchangeName, RabbitMqConstants.RoutingKey, null, messageBodyBytes);

        channel.Close();
        cnn.Close();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
