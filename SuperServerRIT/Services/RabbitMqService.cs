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
            _model.QueueDeclare(queue: "equipment_status", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void SendMessage(string message)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(message);
                _model.BasicPublish(exchange: "", routingKey: "equipment_status", basicProperties: null, body: body);
                Console.WriteLine($"[x] Отправлено {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке сообщения в RabbitMQ: {ex.Message}");
            }
        }

        public void ReceiveMessages(Action<string> onMessageReceived)
        {
            var consumer = new EventingBasicConsumer(_model);
            consumer.Received += (channel, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[x] Получено: {message}");

                try
                {
                    onMessageReceived?.Invoke(message);
                 
                    _model.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при обработке сообщения: {ex.Message}");
                   
                }
            };

            try
            {
                _model.BasicConsume(queue: "equipment_status", autoAck: false, consumer: consumer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении сообщения из RabbitMQ: {ex.Message}");
            }
        }


        public void Dispose()
        {
            if (_model?.IsOpen ?? false)
            {
                _model.Close();
            }
            _model?.Dispose();
            _connection?.Dispose();
            Console.WriteLine("RabbitMQ соединение закрыто.");
        }

    }
}
