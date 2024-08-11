internal class RabbitMqConstants
{
    public static string Uri = "amqp://guest:guest@rabbitmq:5672";
    public const string ClientProvidedName = "Rabbit Sender App Login";
    public const string GerResponseFromServerQueueName = "Rabbit Reciver App Login";
    public const string ExchangeName = "Exchange";
    public const string RoutingKey = "routhing-key";
    public const string QueueName = "Queue";
}