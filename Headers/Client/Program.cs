using System;
using System.Collections.Generic;

namespace Client
{
    class Program
    {
        private static Dictionary<string, string> ColorDictionary => new Dictionary<string, string>()
        {
            { "0", "green"},
            { "1", "green"},
            { "2", "green"},
            { "3", "red"},
            { "4", "none"},
            { "-1", "green"}
        };

        private static Dictionary<string, string> TreeDictionary => new Dictionary<string, string>()
        {
            { "0", "eucalyptus"},
            { "1", "none"},
            { "2", "aloe-vera"},
            { "3", "eucalyptus"},
            { "4", "none"},
            { "-1", "aloe-vera"}
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
                    var message = $"Message: {messageCount}";

                    publisher.PublishMessage(message, GetDictionary(messageCount));

                    messageCount++;
                }
            }

            Console.ReadLine();
        }

        private static Dictionary<string, string> GetDictionary(int messageCount)
        {
            if (!ColorDictionary.TryGetValue(messageCount.ToString(), out var color))
                color = ColorDictionary["-1"];
            if (!TreeDictionary.TryGetValue(messageCount.ToString(), out var tree))
                tree = TreeDictionary["-1"];
            return new Dictionary<string, string>() {
                { "color", color },
                { "tree", tree }
            };
        }
    }
}
