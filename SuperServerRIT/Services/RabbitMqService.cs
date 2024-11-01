using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;

namespace SuperServerRIT.Services
{
    public class RabbitMqService : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string QueueName = "equipment_status";

        public RabbitMqService()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void SendMessage<T>(T message)
        {
            try
            {
                string messageJson = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(messageJson);
                _channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: null, body: body);
                Console.WriteLine($"[x] Sent: {messageJson}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to RabbitMQ: {ex.Message}");
            }
        }

        public void ReceiveMessages(Func<string, Task> onMessageReceivedAsync)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[x] Received: {message}");

                try
                {
                    await onMessageReceivedAsync(message);
                    _channel.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            };

            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
        }

        public void Dispose()
        {
            if (_channel?.IsOpen ?? false)
            {
                _channel.Close();
            }
            _channel?.Dispose();
            _connection?.Dispose();
            Console.WriteLine("RabbitMQ connection closed.");
        }
    }
}
