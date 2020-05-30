using Contract;
using RabbitMQ.Client;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace Client
{
    public class RabbitPublisher : IDisposable
    {
        #region Properties
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private const string QueueName = "OneWayMessaging.Sample3";
        private const string ExchangeName = "";
        public const bool IsDurable = false;
        public const string VirtualHost = "";
        public int Port = 0;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _model;
        #endregion

        #region Ctor.
        public RabbitPublisher()
        {
            DisplaySettings();
            SetupRabbitMq();
        }
        #endregion

        #region Public_Methods
        public void PublishMessage(object message, MessageFormat format)
        {
            var properties = _model.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = GetContentType(format);
            properties.Type = GetMessageType(message);

            byte[] myMessage = SerializeMessage(message, format);

            _model.BasicPublish(ExchangeName, QueueName, properties, myMessage);
        }

        public void Dispose()
        {
            if (_connection != null)
                _connection.Dispose();
            if (_model != null)
                _model.Dispose();
        }
        #endregion

        #region Private_Methods
        private void SetupRabbitMq()
        {
            _connectionFactory = new ConnectionFactory()
            {
                HostName = HostName,
                UserName = UserName,
                Password = Password
            };

            if (!string.IsNullOrEmpty(VirtualHost))
                _connectionFactory.VirtualHost = VirtualHost;

            if (Port > 0)
                _connectionFactory.Port = Port;

            _connection = _connectionFactory.CreateConnection();
            _model = _connection.CreateModel();
        }

        private void DisplaySettings()
        {
            Console.WriteLine("Host: {0}", HostName);
            Console.WriteLine("UserName: {0}", UserName);
            Console.WriteLine("Password: {0}", Password);
            Console.WriteLine("QueueName: {0}", QueueName);
            Console.WriteLine("ExchangeName: {0}", ExchangeName);
            Console.WriteLine("VirtualHost: {0}", VirtualHost);
            Console.WriteLine("Port: {0}", Port);
            Console.WriteLine("IsDurable: {0}", IsDurable);
        }

        private static string GetContentType(MessageFormat format)
        {
            var contentType = string.Empty;

            if (format.Equals(MessageFormat.Json))
                contentType = "application/json";
            else if (format.Equals(MessageFormat.Xml))
                contentType = "text/xml";
            else if (format.Equals(MessageFormat.Binary))
                contentType = "application/octet-stream";

            return contentType;
        }

        private static string GetMessageType(object messageObject)
        {
            return messageObject.GetType().Name;
        }

        private static byte[] SerializeMessage(object myMessage, MessageFormat format)
        {
            byte[] result = null;

            if (format.Equals(MessageFormat.Json))
            {
                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(myMessage);
                result = Encoding.Default.GetBytes(jsonString);
            }
            else if (format.Equals(MessageFormat.Xml))
            {
                var messageStream = new MemoryStream();
                var xmlSerializer = new XmlSerializer(myMessage.GetType());
                xmlSerializer.Serialize(messageStream, myMessage);
                messageStream.Flush();
                messageStream.Seek(0, SeekOrigin.Begin);
                result = messageStream.GetBuffer();
            }
            else if (format.Equals(MessageFormat.Binary))
            {
                var messageStream = new MemoryStream();
                var binarySerializer = new BinaryFormatter();
                binarySerializer.Serialize(messageStream, myMessage);
                messageStream.Flush();
                messageStream.Seek(0, SeekOrigin.Begin);
                result = messageStream.GetBuffer();
            }

            return result;
        }
        #endregion
    }
}