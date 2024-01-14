using Rabbit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

ConnectionFactory _factory = new ConnectionFactory();
_factory.Uri = new("amqps://kcvjurgs:DvkvqDRBS6554zJO3_Uxw9L2PvATDtWp@sparrow.rmq.cloudamqp.com/kcvjurgs");

IConnection _conn = _factory.CreateConnection();
IModel _channel = _conn.CreateModel();

// Connect
string connectQueue = RabbitConsts.GetConnectedInfoQueue();

_channel.QueueDeclare(queue: connectQueue, durable: true, exclusive: false, autoDelete: false);


// Test
string testQueueName = RabbitConsts.StartTestQueue();
await Task.Run(() => _channel.QueueDeclare(queue: testQueueName, durable: false, exclusive: false, autoDelete: true));

// Back Up
string exchange = RabbitConsts.SendBackUpInfoExchange();

_channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Fanout, durable: true, autoDelete: false, arguments: null);

string backUpQueueName = _channel.QueueDeclare().QueueName;

_channel.QueueBind(queue: backUpQueueName, exchange: exchange, routingKey: string.Empty);

string? userInput = "start";

while (userInput != "x")
{
    Console.Write("Consumer Input: ");
    userInput = Console.ReadLine();

    switch (userInput)
    {
        case "connect":
            PublishConnectedInfo("test");
            continue;

        case "test":
            ConsumeStartTest();
            continue;

        case "back":
            ConsumeBackUpInfo();
            continue;

        case "clear":
            Console.Clear();
            continue;

        default:
            break;
    }
}

//ConnectionFactory factory = new();
//factory.Uri = new("amqps://kcvjurgs:DvkvqDRBS6554zJO3_Uxw9L2PvATDtWp@sparrow.rmq.cloudamqp.com/kcvjurgs");

//using IConnection con = factory.CreateConnection();
//using IModel channel = con.CreateModel();

//await ConsumeStartTest();
//Console.Read();

//async Task ConsumeStartTest()
//{
//    await Task.Run(() => Console.WriteLine());
//    string queueName = "test-queue-2";
//    channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: true);

//    EventingBasicConsumer consumer = new(channel);

//    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
//    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

//    consumer.Received += async (sender, e) =>
//    {
//        await Task.Delay(200);
//        Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
//    };
//}






void PublishConnectedInfo(string serviceName)
{
    string message = RabbitConsts.GetConnectedInfo(serviceName);
    byte[] body = Encoding.UTF8.GetBytes(message);

    _channel.BasicPublish(exchange: string.Empty, routingKey: connectQueue, body: body);
}

void ConsumeBackUpInfo()
{

    EventingBasicConsumer consumer = new(_channel);
    _channel.BasicConsume(queue: backUpQueueName, autoAck: false, consumer: consumer);

    consumer.Received += (sender, e) => Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
}

async Task ConsumeStartTest()
{
    EventingBasicConsumer consumer = new(_channel);

    _channel.BasicConsume(queue: testQueueName, autoAck: true, consumer: consumer);
    _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

    consumer.Received += async (sender, e) =>
    {
        await Task.Delay(200);
        Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
    };
}