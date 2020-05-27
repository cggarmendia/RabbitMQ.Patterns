using RabbitMQ.Client;
using System;
using System.Text;

namespace Client
{
    public class RabbitPublisher : IDisposable
    {
        #region Properties
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private const string QueueName = "";
        private const string ExchangeName = "Topic.Sample7.Exchange";
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
        #endregion

        #region Public_Methods
        public void PublishMessage(string message, string routingKey)
        {
            var properties = _model.CreateBasicProperties();
            properties.Persistent = true;

            byte[] myMessage = Encoding.Default.GetBytes(message);

            _model.BasicPublish(ExchangeName, routingKey, properties, myMessage);
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
