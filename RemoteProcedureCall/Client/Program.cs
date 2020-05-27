using System;

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

                if (key.Key != ConsoleKey.Enter)
                    continue;

                var message = $"Message: {messageCount}";

                Console.WriteLine("Sending - {0}", messageCount);

                var response = publisher.PublishMessage(message, new TimeSpan(0,0,3,0));

                Console.WriteLine("Response - {0}", response);

                messageCount++;
            }

            Console.ReadLine();
        }
    }
}
