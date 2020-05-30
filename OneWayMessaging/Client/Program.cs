using Contract;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace Client
{
    class Program
    {
        private static Dictionary<string, string> FormatDictionary => new Dictionary<string, string>()
        {
            { "0", "json"},
            { "1", "xml"},
            { "2", "binary"},
            { "3", "xml"},
            { "4", "xml"},
            { "-1", "json"}
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
                    var myMessage = new MyMessage()
                    {
                        Message = $"Message: {messageCount}"
                    };

                    Console.WriteLine("Sending - {0}", myMessage.Message);

                    publisher.PublishMessage(myMessage, GetMessageFormat(messageCount));

                    messageCount++;
                }
            }
            
            Console.ReadLine();
        }

        private static MessageFormat GetMessageFormat(int messageCount) 
        {
            var result = MessageFormat.None;

            var format = GetFormat(messageCount).ToLower();
            if (format.Equals("json"))
                result = MessageFormat.Json;
            else if (format.Equals("xml"))
                result = MessageFormat.Xml;
            else if (format.Equals("binary"))
                result = MessageFormat.Binary;

            return result;
        }

        private static string GetFormat(int messageCount)
        {
            if (!FormatDictionary.TryGetValue(messageCount.ToString(), out var format))
                format = FormatDictionary["-1"];
            return format;
        }
    }
}
