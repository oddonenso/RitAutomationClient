using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Data.Tables;
using Microsoft.EntityFrameworkCore;
using SuperServerRIT.Model;
using System.Text.Json;

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
            if (string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine("Ошибка: Получено пустое сообщение.");
                return;
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<Connection>();

                try
                {
                    var equipmentData = JsonSerializer.Deserialize<EquipmentMessage>(message);

                    if (equipmentData == null)
                    {
                        Console.WriteLine("Ошибка: Неправильный формат сообщения.");
                        return;
                    }

                    var equipment = new Equipment
                    {
                        Name = equipmentData.Name,
                        Type = equipmentData.Type,
                        Status = equipmentData.Status
                    };

                    dbContext.Equipment.Add(equipment);
                    await dbContext.SaveChangesAsync();

                    Console.WriteLine($"Сохранено оборудование: {equipment.Name}");
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Ошибка десериализации JSON: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при обработке сообщения: {ex.Message}");
                }
            }
        }

    }


}
