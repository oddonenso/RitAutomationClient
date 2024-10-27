using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Data.Tables;
using Microsoft.EntityFrameworkCore;

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
            _rabbitMqService.ReceiveMessages(OnMessageReceived);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _rabbitMqService.Dispose();
            return Task.CompletedTask;
        }

        private async void OnMessageReceived(string message)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<Connection>();

                try
                {
                    // Логика обработки сообщений об оборудовании
                    var equipment = new Equipment
                    {
                        Name = message, // Здесь должно быть нужное преобразование строки
                        Type = "ExampleType" // Укажите тип или извлеките его из сообщения
                    };

                    dbContext.Equipment.Add(equipment);
                    await dbContext.SaveChangesAsync();
                    Console.WriteLine($"Сохранено оборудование: {equipment.Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при обработке сообщения: {ex.Message}");
                }
            }
        }
    }
}
