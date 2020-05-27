using System;
using System.Globalization;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting RabbitMQ Message Publisher");
            Console.WriteLine();
            Console.WriteLine();

            var messageCount = 0;
            var publisher = new RabbitPublisher();

            Console.WriteLine("Enter key to publish a message.");

            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q)
                    break;

                if (key.Key == ConsoleKey.Enter)
                {
                    var routingKey = new Random().Next(0, 4).ToString(CultureInfo.InvariantCulture);

                    var message = $"Message: {messageCount}";
                    Console.WriteLine("Sending: {0} - Routing Key: {1}", messageCount, routingKey);

                    publisher.PublishMessage(message, routingKey);

                    messageCount++;
                }
            }

            Console.ReadLine();
        }
    }
}
