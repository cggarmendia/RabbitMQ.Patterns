using Contract;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace Server
{
    public class RabbitConsumer : IDisposable
    {
        #region Properties
        private static Dictionary<string, Type> MessageTypeDictionary => new Dictionary<string, Type>()
        {
            { "MyMessage", typeof(MyMessage)},
            { "MyOtherMessage", typeof(MyOtherMessage)},
        };
        public bool Enable { get; set; }
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
        public RabbitConsumer()
        {
            DisplaySettings();
            SetupRabbitMq();
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
            _model.BasicQos(0, 1, false);
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
            Console.WriteLine("Enable: {0}", Enable);
        }

        private void OnRecieved(object sender, BasicDeliverEventArgs deliveryArgs)
        {
            var messageFormat = GetMessageFormat(deliveryArgs.BasicProperties.ContentType);
            Console.WriteLine("Message Recieved - Message Content Type = {0}", deliveryArgs.BasicProperties.ContentType);

            var messageAsString = GetMessageAsString(deliveryArgs.Body, messageFormat);
            Console.WriteLine("Message Recieved - Message String = {0}", messageAsString);

            var messageType = deliveryArgs.BasicProperties.Type;
            Console.WriteLine("Message Recieved - Message Type = {0}", messageType);

            var myUnTypedMessage = DeserializeMessage(deliveryArgs.Body, messageAsString, messageFormat, messageType);

            if (myUnTypedMessage.GetType().Equals(typeof(MyMessage)))
            {
                var myMessage = myUnTypedMessage as MyMessage;
                Console.WriteLine("Data from object: MyMessage = {0}", myMessage.Message);
            }
            else if (myUnTypedMessage.GetType().Equals(typeof(MyOtherMessage)))
            {
                var myMessage = myUnTypedMessage as MyOtherMessage;
                Console.WriteLine("Data from object: MyOtherMessage = {0}", myMessage.Message);
            }
            else
            {
                Console.WriteLine("Data from object: Unknow data type");
            }


            _model.BasicAck(deliveryArgs.DeliveryTag, false);
        }
        #endregion

        #region Public_Methods
        public void Start()
        {
            var consumer = new EventingBasicConsumer(_model);
            consumer.Received += OnRecieved;
            _model.BasicConsume(QueueName, false, consumer);
        }

        public void Dispose()
        {
            if (_connection != null)
                _connection.Dispose();
            if (_model != null)
                _model.Dispose();
        }

        private static MessageFormat GetMessageFormat(string contentType)
        {
            var format = MessageFormat.None;

            if (contentType.Equals("application/json"))
                format = MessageFormat.Json;
            else if (contentType.Equals("text/xml"))
                format = MessageFormat.Xml;
            else if (contentType.Equals("application/octet-stream"))
                format = MessageFormat.Binary;

            return format;
        }

        private static string GetMessageAsString(byte[] body, MessageFormat messageFormat)
        {
            var messageAsString = string.Empty;

            if (messageFormat.Equals(MessageFormat.Json))
                messageAsString = Encoding.Default.GetString(body);
            else if (messageFormat.Equals(MessageFormat.Xml))
                messageAsString = Encoding.Default.GetString(body);
            else if (messageFormat.Equals(MessageFormat.Binary))
                messageAsString = Convert.ToBase64String(body);

            return messageAsString;
        }

        private static object DeserializeMessage(byte[] body, string messageAsString, MessageFormat format, string messageType)
        {
            object result = null;

            if (format.Equals(MessageFormat.Json))
            {
                result = Newtonsoft.Json.JsonConvert.DeserializeObject(messageAsString, MessageTypeDictionary[messageType]);
            }
            else if (format.Equals(MessageFormat.Xml))
            {
                var messageStream = new MemoryStream();
                messageStream.Write(body, 0, body.Length);
                messageStream.Seek(0, SeekOrigin.Begin);
                var xmlSerializer = new XmlSerializer(MessageTypeDictionary[messageType]);
                result = xmlSerializer.Deserialize(messageStream);
            }
            else if (format.Equals(MessageFormat.Binary))
            {
                var messageStream = new MemoryStream();
                messageStream.Write(body, 0, body.Length);
                messageStream.Seek(0, SeekOrigin.Begin);
                var binarySerializer = new BinaryFormatter();
                if (messageType.Contains(nameof(MyOtherMessage)))
                    result = binarySerializer.Deserialize(messageStream) as MyOtherMessage;
                else if (messageType.Contains(nameof(MyMessage)))
                    result = binarySerializer.Deserialize(messageStream) as MyMessage;
            }

            return result;
        }
        #endregion
    }
}
