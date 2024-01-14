using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Rabbit
{
    public interface IMessagePublisherService
    {
        public void ConsumeConnectedInfo();
        public void PublishBackUpInfo();
        public void PublishStartTest();
    }

    public class MessagePublisherService : IMessagePublisherService
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _conn;
        private readonly IModel _channel;

        public MessagePublisherService()
        {
            _factory = new ConnectionFactory();
            _factory.Uri = new("amqps://kcvjurgs:DvkvqDRBS6554zJO3_Uxw9L2PvATDtWp@sparrow.rmq.cloudamqp.com/kcvjurgs");

            _conn = _factory.CreateConnection();
            _channel = _conn.CreateModel();
        }

        public void ConsumeConnectedInfo()
        {
            string queue = RabbitConsts.GetConnectedInfoQueue();
            EventingBasicConsumer consumer = new(_channel);

            _channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false);
            _channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);

            consumer.Received += (sender, e) => Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
        }

        public void PublishBackUpInfo()
        {
            string exchange = RabbitConsts.SendBackUpInfoExchange();
            string message = RabbitConsts.SendBackUpInfo();
            byte[] body = Encoding.UTF8.GetBytes(message);

            _channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Fanout, durable: true, autoDelete: false, arguments: null);
            _channel.BasicPublish(exchange: exchange, routingKey: string.Empty, body: body);
        }

        public void PublishStartTest()
        {
            string queueName = RabbitConsts.StartTestQueue();
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: true);

            for (int count = 1; count <= 100; count++)
            {
                string message = RabbitConsts.StartTest(count);
                byte[] body = Encoding.UTF8.GetBytes(message);

                _channel.BasicPublish(exchange: string.Empty, routingKey: queueName, body: body);
            }
        }
    }
}
