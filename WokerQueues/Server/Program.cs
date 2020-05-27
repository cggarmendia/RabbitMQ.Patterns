using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting  RabbitMQ Message Consumer");
            Console.WriteLine();
            Console.WriteLine();

            var consumer = new RabbitConsumer() { Enable = true };
            consumer.Start();
            Console.ReadLine();
        }
    }
}
