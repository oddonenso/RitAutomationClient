using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace SuperServerRIT.Services
{
    public class RabbitMqService
    {
        private readonly IConnection _connection;
        private IModel _model;


        public RabbitMqService()
        {
            //настройка фабрики для коннекта к RabbitMq
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };

            //create connect

            _connection = factory.CreateConnection();
            _model = _connection.CreateModel();

            //create queue

            _model.QueueDeclare(queue: "equipment_status",
                durable: false,
                autoDelete: false,
                exclusive: false,
                arguments: null);
        }

        //method for publish message

        public void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _model.BasicPublish(exchange: "", routingKey: "equipment_status", basicProperties: null,
                body: body);
            Console.WriteLine($"[x] Отправлено {message}");
        }

       
        //method for subscribe message in list with callback
        public void ReceiveMessages(Action<string> onMessageReceived)
        {
            var consumer = new EventingBasicConsumer(_model);
            consumer.Received += (channel, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[x] Received {message}");
                onMessageReceived?.Invoke(message);  // Вызов коллбека
            };
            _model.BasicConsume(queue: "equipment_status",
                autoAck: true,
                consumer: consumer);
        }


        public void Dispose()
        {
            _connection?.Dispose();
            _model.Close();
        }
    }
}
