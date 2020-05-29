using Contract;
using RabbitMQ.Client;
using System;
using System.Text;

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
                    var myMessage = new MyMessage() 
                    {
                        Message = $"Message: {messageCount}"
                    };

                    Console.WriteLine("Sending - {0}", myMessage.Message);

                    publisher.PublishMessage(myMessage);

                    messageCount++;
                }
            }
            
            Console.ReadLine();
        }

    }
}
