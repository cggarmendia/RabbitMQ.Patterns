using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Server2
{
    public class RabbitConsumer : IDisposable
    {
        #region Properties
        public bool Enable { get; set; }
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private const string QueueName = "WokerQueues.Sample4";
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
            var message = Encoding.Default.GetString(deliveryArgs.Body);

            Console.WriteLine("Message Recieved - {0}", message);

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
        #endregion
    }
}
