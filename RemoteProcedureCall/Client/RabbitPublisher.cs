using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Client
{
    internal class RabbitPublisher : IDisposable
    {
        #region Properties
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private const string QueueName = "RemoteProcedureCall.Sample6";
        private const string ExchangeName = "";
        public const bool IsDurable = false;
        public const string VirtualHost = "";
        public int Port = 0;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _model;
        private EventingBasicConsumer _consumer;
        private string _responseQueue;
        private string _correlationId;
        private string _response;
        private bool _onRecievedFinished;
        #endregion

        #region Ctor
        public RabbitPublisher()
        {
            DisplaySettings();
            SetupRabbitMq();
        }
        #endregion

        #region Private_Methods
        private void OnRecieved(object sender, BasicDeliverEventArgs deliveryTags)
        {
            if (deliveryTags.BasicProperties != null &&
                deliveryTags.BasicProperties.CorrelationId == _correlationId)
            {
                _response = Encoding.Default.GetString(deliveryTags.Body);
                _onRecievedFinished = true;
            }
        }

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

            //Creating dynamic response queue
            _responseQueue = _model.QueueDeclare().QueueName;
            _consumer = new EventingBasicConsumer(_model);
            _model.BasicConsume(_responseQueue, true, _consumer);
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
        public string PublishMessage(string message, TimeSpan timeout)
        {
            _correlationId = Guid.NewGuid().ToString();
            _onRecievedFinished = false;

            var properties = _model.CreateBasicProperties();
            properties.ReplyTo = _responseQueue;
            properties.CorrelationId = _correlationId;

            byte[] myMessage = Encoding.Default.GetBytes(message);

            var timeoutAt = DateTime.Now + timeout;

            _model.BasicPublish(ExchangeName, QueueName, properties, myMessage);

            while (DateTime.Now <= timeoutAt && !_onRecievedFinished)
            {
                _consumer.Received += OnRecieved;
            }

            if (timeoutAt <= DateTime.Now)
                throw new TimeoutException($"Te response was not returned before the timeout: {timeout}");

            return _response;
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
