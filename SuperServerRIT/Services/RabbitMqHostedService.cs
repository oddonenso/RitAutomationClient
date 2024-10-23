using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Data.Tables;

namespace SuperServerRIT.Services
{
    public class RabbitMqHostedService : IHostedService
    {
        private readonly RabbitMqService _rabbitMqService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMqHostedService(RabbitMqService rabbitMqService, IServiceScopeFactory serviceScopeFactory)
        {
            _rabbitMqService = rabbitMqService;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Запускаем подписку на получение сообщений в фоне
            _rabbitMqService.ReceiveMessages(OnMessageReceived);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Закрытие ресурсов при остановке
            _rabbitMqService.Dispose();
            return Task.CompletedTask;
        }

        // Этот метод будет вызван при получении сообщения от RabbitMQ
        private async void OnMessageReceived(string message)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<Connection>();

                // Пример записи сообщения в БД
                var equipmentStatus = new EquipmentStatus
                {
                    Status = message,
                    Timestamp = DateTime.UtcNow
                };

                dbContext.EquipmentStatus.Add(equipmentStatus);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
