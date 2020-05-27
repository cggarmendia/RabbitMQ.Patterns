using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server2
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
