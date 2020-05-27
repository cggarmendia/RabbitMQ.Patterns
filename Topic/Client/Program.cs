using System;
using System.Collections.Generic;
using System.Globalization;

namespace Client
{
    class Program
    {
        private static Dictionary<string, string>  RoutingKeyDictionary => new Dictionary<string, string>()
        {
            { "0", "world.cuba.la-habana"},
            { "1", "world.cuba.matanzas"},
            { "2", "universal.cuba.matanzas"},
            { "3", "world.universal.cuba.matanzas"},
            { "4", "www.universal.cuba.la-habana"},
            { "-1", "world.cuba.la-habana"}
        };

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
                    var routingKey = GetRoutingKey(messageCount);

                    var message = $"Message: {messageCount}";
                    Console.WriteLine("Sending: {0} - Routing Key: {1}", messageCount, routingKey);

                    publisher.PublishMessage(message, routingKey);

                    messageCount++;
                }
            }

            Console.ReadLine();
        }

        private static string GetRoutingKey(int messageCount)
        {
            if (!RoutingKeyDictionary.TryGetValue(messageCount.ToString(), out var routingKey))
                routingKey = RoutingKeyDictionary["-1"];
            return routingKey;
        }
    }
}
