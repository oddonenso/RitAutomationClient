using Data.Tables;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace SuperServerRIT.Services
{
    public class RabbitMqService
    {
        private readonly IConnection _connection;
        private readonly IModel _model;

        public RabbitMqService()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };

            _connection = factory.CreateConnection();
            _model = _connection.CreateModel();
            _model.QueueDeclare(queue: "equipment_status", durable: false, autoDelete: false, exclusive: false, arguments: null);
        }

      public void SendMessage(string message)
{
    var body = Encoding.UTF8.GetBytes(message);
    _model.BasicPublish(exchange: "", routingKey: "equipment_status", basicProperties: null, body: body);
    Console.WriteLine($"[x] Отправлено {message}");
}


        public void ReceiveMessages(Action<string> onMessageReceived)
        {
            var consumer = new EventingBasicConsumer(_model);
            consumer.Received += (channel, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[x] Получено: {message}");
                onMessageReceived?.Invoke(message);
            };
            _model.BasicConsume(queue: "equipment_status", autoAck: true, consumer: consumer);
        }

        public void Dispose()
        {
            _model?.Close();
            _connection?.Dispose();
        }
    }
}
