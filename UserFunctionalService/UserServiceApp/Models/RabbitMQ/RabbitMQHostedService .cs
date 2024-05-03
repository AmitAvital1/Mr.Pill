using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class RabbitMQHostedService : IHostedService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQHostedService()
    {
        bool example = false;
        
        if (example)
        {
               var factory = new ConnectionFactory() { HostName = "localhost" };
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(queue: "my_queue",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
        }
     
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        bool example = false;

        if (example)
        {
              var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Received message: {0}", message);
                // here i need to save the massage to the db !!
            };

            _channel.BasicConsume(queue: "my_queue",
                                autoAck: true,
                                consumer: consumer);

        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        bool example = false;

        if (example)
        {
            _channel.Close();
            _connection.Close();
        }

        return Task.CompletedTask;
    }
}
