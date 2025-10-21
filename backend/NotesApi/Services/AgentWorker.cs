using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NotesApi.Data;
using NotesApi.Models;
using System.Text.Json;

namespace NotesApi.Services
{
    public class AgentWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AgentWorker> _logger;
        private readonly TimeSpan _delay = TimeSpan.FromSeconds(10);

        public AgentWorker(IServiceScopeFactory scopeFactory, ILogger<AgentWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AgentWorker iniciado...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var planner = scope.ServiceProvider.GetRequiredService<PlannerService>();
                    var executor = scope.ServiceProvider.GetRequiredService<ExecutorService>();

                    var pendingTasks = await context.AgentTasks
                        .Where(t => t.Status == "pending")
                        .OrderBy(t => t.CreatedAt)
                        .ToListAsync(stoppingToken);

                    foreach (var task in pendingTasks)
                    {
                        try
                        {
                            task.Status = "running";
                            await context.SaveChangesAsync(stoppingToken);

                            var planJson = planner.GeneratePlan(task.Prompt);
                            task.PlanJson = planJson;
                            await context.SaveChangesAsync(stoppingToken);

                            var steps = planner.ParsePlan(planJson);
                            var results = await executor.ExecutePlan(int.Parse(task.UserId), steps);

                            foreach (var r in results)
                            {
                                context.AgentAuditLogs.Add(new AgentAuditLog
                                {
                                    AgentTaskId = task.Id,
                                    StepIndex = r.StepIndex,
                                    Action = r.Action,
                                    ParametersJson = "{}", // opcional
                                    Result = r.Success ? "success" : "failed",
                                    Message = r.Message
                                });
                            }

                            task.Status = "done";
                            task.CompletedAt = DateTime.UtcNow;
                            await context.SaveChangesAsync(stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error ejecutando tarea AgentTask {task.Id}");
                            task.Status = "failed";
                            await context.SaveChangesAsync(stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en ciclo AgentWorker");
                }

                await Task.Delay(_delay, stoppingToken);
            }

            _logger.LogInformation("AgentWorker detenido.");
        }
    }
}
