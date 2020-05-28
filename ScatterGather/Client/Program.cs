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

            Console.WriteLine("Enter a number 1-6 then press enter key to send a message.");
            int messageNumber = 0;
            while (true)
            {
                var key = Console.ReadKey();

                ProcessKeyStroke(ref messageNumber, ref key);

                if (key.Key.Equals(ConsoleKey.Enter)) 
                {
                    if (messageNumber == 0)
                    {
                        Console.WriteLine("Please supply a number from 1 - 6 and press enter key.");
                        continue;
                    }

                    var message = $"Message ID: {messageCount}";
                    Console.WriteLine("Sending - {0}", message);

                    var responses = publisher.PublishMessage(message, messageNumber.ToString(), new TimeSpan(0, 0, 0, 0, 400), 3);

                    Console.WriteLine();
                    Console.WriteLine("{0} replies recivied", responses.Count);
                    Console.WriteLine();
                    Console.WriteLine("Listing responses.");
                    foreach (var response in responses)
                        Console.WriteLine("Response - {0}", response);

                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Please supply a number from 1 - 6 and press enter key.");

                    messageCount++;
                }
            }
        }

        private static void ProcessKeyStroke(ref int messageNumber, ref ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.D1:
                    messageNumber = 1;
                    break;
                case ConsoleKey.D2:
                    messageNumber = 2;
                    break;
                case ConsoleKey.D3:
                    messageNumber = 3;
                    break;
                case ConsoleKey.D4:
                    messageNumber = 4;
                    break;
                case ConsoleKey.D5:
                    messageNumber = 5;
                    break;
                case ConsoleKey.D6:
                    messageNumber = 6;
                    break;
                case ConsoleKey.Enter:
                    break;
                default:
                    messageNumber = 0;
                    break;
            }
        }
    }
}
