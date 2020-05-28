using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Client
{
    internal class RabbitPublisher : IDisposable
    {
        #region Properties
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private const string QueueName = "";
        private const string ExchangeName = "ScatterGather.Sample9.Exchange";
        public const bool IsDurable = false;
        public const string VirtualHost = "";
        public int Port = 0;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _model;
        private EventingBasicConsumer _consumer;
        private string _responseQueue;
        private string _correlationId;
        private List<string> _responses;
        private int _minResponses;
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
                var response = Encoding.Default.GetString(deliveryTags.Body);
                Console.WriteLine("Sender got response: {0}", response);
                _responses.Add(response);

                if(_responses.Count == 10)
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
            _consumer.Received += OnRecieved;
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
        public List<string> PublishMessage(string message, string routingKey, TimeSpan timeout, int minResponses)
        {
            _responses = new List<string>();
            _correlationId = Guid.NewGuid().ToString();
            _onRecievedFinished = false;
            _minResponses = minResponses;

            var properties = _model.CreateBasicProperties();
            properties.ReplyTo = _responseQueue;
            properties.CorrelationId = _correlationId;

            byte[] myMessage = Encoding.Default.GetBytes(message);

            var timeoutAt = DateTime.Now + timeout;

            _model.BasicPublish(ExchangeName, routingKey, properties, myMessage);
            
            while (DateTime.Now <= timeoutAt && !_onRecievedFinished)
            {
                if (_responses.Count >= _minResponses)
                    _onRecievedFinished = true;

                Console.WriteLine("Waiting for responses");
                Thread.Sleep(new TimeSpan(0, 0, 0, 0, 400));
            }

            return _responses;
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
