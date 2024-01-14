using Rabbit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;


ConnectionFactory _factory;
IConnection _conn;
IModel _channel;

_factory = new ConnectionFactory();
_factory.Uri = new("amqps://kcvjurgs:DvkvqDRBS6554zJO3_Uxw9L2PvATDtWp@sparrow.rmq.cloudamqp.com/kcvjurgs");

_conn = _factory.CreateConnection();
_channel = _conn.CreateModel();


// Test
string testQueueName = RabbitConsts.StartTestQueue();
_channel.QueueDeclare(queue: testQueueName, durable: false, exclusive: false, autoDelete: true);

// Back Up
string backUpExchange = RabbitConsts.SendBackUpInfoExchange();
_channel.ExchangeDeclare(exchange: backUpExchange, type: ExchangeType.Fanout, durable: true, autoDelete: false, arguments: null);

// Connected
string connectedQueue = RabbitConsts.GetConnectedInfoQueue();
_channel.QueueDeclare(queue: connectedQueue, durable: true, exclusive: false, autoDelete: false);

string? userInput = "start";

while (userInput != "x")
{
    Console.Write("Publisher Input: ");
    userInput = Console.ReadLine();

    switch (userInput)
    {
        case "test":
            PublishStartTest();
            continue;

        case "back":
            PublishBackUpInfo();
            continue;

        case "connected":
            ConsumeConnectedInfo();
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

//channel.QueueDeclare(queue: "TestQueue1", durable: true, exclusive: false, autoDelete: false);

//byte[] message = Encoding.UTF8.GetBytes("MESAJJJ");
//channel.BasicPublish(exchange: "", routingKey: "TestQueue1", body: message);

//Console.Read();





void ConsumeConnectedInfo()
{
    EventingBasicConsumer consumer = new(_channel);

    _channel.BasicConsume(queue: connectedQueue, autoAck: true, consumer: consumer);

    consumer.Received += (sender, e) => Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
}

void PublishBackUpInfo()
{
    string message = RabbitConsts.SendBackUpInfo();
    byte[] body = Encoding.UTF8.GetBytes(message);

    _channel.BasicPublish(exchange: backUpExchange, routingKey: string.Empty, body: body);
}

void PublishStartTest()
{

    for (int count = 1; count <= 100; count++)
    {
        string message = RabbitConsts.StartTest(count);
        byte[] body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: string.Empty, routingKey: testQueueName, body: body);
    }
}