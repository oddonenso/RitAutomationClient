using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Data;
using System.Text.Json;
using SuperServerRIT.Model;

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
            _rabbitMqService.ReceiveMessages(OnMessageReceivedAsync);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _rabbitMqService.Dispose();
            return Task.CompletedTask;
        }

        private async Task OnMessageReceivedAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine("Error: Received an empty message.");
                return;
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<Connection>();

                try
                {
                    var equipmentData = JsonSerializer.Deserialize<EquipmentMessage>(message);

                    if (equipmentData?.Action == "UpdateStatus")
                    {
                        var equipment = await dbContext.Equipment.FindAsync(equipmentData.EquipmentId);
                        if (equipment != null)
                        {
                            equipment.Status = equipmentData.Status;
                            await dbContext.SaveChangesAsync();
                            Console.WriteLine($"Updated status for equipment {equipment.Name} to {equipment.Status}");
                        }
                        else
                        {
                            Console.WriteLine("Equipment not found.");
                        }
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON deserialization error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            }
        }
    }
}
