using RabbitMQ.Client;
using System;
using System.Text;

namespace Client
{
    class Program
    {
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private const string QueueName = "Sample2";
        private const string ExchangeName = "";

        static void Main(string[] args)
        {
            PublishMessageFromDefaultExhangeToSpecificQueue(true);
        }

        private static void CreateDefaultQueueAndExchange()
        {
            Console.WriteLine("Starting  RabbitMQ Queue Creator");
            Console.WriteLine();
            Console.WriteLine();

            var connectionFactory = new ConnectionFactory() { HostName = HostName, UserName = UserName, Password = Password };
            var connection = connectionFactory.CreateConnection();
            var model = connection.CreateModel();

            model.QueueDeclare("MyQueue", true, false, false, null);
            Console.WriteLine("Queue created");

            model.ExchangeDeclare("MyExchange", ExchangeType.Topic);
            Console.WriteLine("Exchange created");

            model.QueueBind("MyQueue", "MyExchange", "cars");
            Console.WriteLine("Exchange and queue bound");

            Console.ReadLine();
        }

        private static void PublishMessageFromDefaultExhangeToSpecificQueue(bool isPersitent)
        {
            Console.WriteLine("Starting  RabbitMQ Queue Creator");
            Console.WriteLine();
            Console.WriteLine();

            var connectionFactory = new ConnectionFactory() { HostName = HostName, UserName = UserName, Password = Password };
            var connection = connectionFactory.CreateConnection();
            var model = connection.CreateModel();

            var properties = model.CreateBasicProperties();
            properties.Persistent = isPersitent;

            byte[] myMessage = Encoding.Default.GetBytes("My message");

            model.BasicPublish(ExchangeName, QueueName, properties, myMessage);

            Console.WriteLine("Message published.");
            Console.ReadLine();
        }
    }
}
