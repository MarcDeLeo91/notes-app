using Microsoft.Extensions.Hosting;
using NotesApi.Data;
using Microsoft.EntityFrameworkCore;

namespace NotesApi.Services
{
    public class AgentWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AgentWorker> _logger;

        public AgentWorker(IServiceProvider serviceProvider, ILogger<AgentWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AgentWorker iniciado.");
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var pending = await db.AgentTasks
                    .Where(t => t.Status == "pending")
                    .OrderBy(t => t.CreatedAt)
                    .FirstOrDefaultAsync(stoppingToken);

                if (pending != null)
                {
                    pending.Status = "running";
                    await db.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation($"Procesando tarea: {pending.Prompt}");

                    try
                    {
                        await Task.Delay(1000, stoppingToken); // Simulación
                        pending.Status = "completed";
                        pending.CompletedAt = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error en tarea del agente");
                        pending.Status = "failed";
                    }

                    await db.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
