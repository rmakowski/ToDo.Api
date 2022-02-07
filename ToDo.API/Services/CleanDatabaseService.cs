using Microsoft.EntityFrameworkCore;
using ToDo.API.Contexts;
using ToDo.API.Entities;
using ToDo.API.Models;

namespace ToDo.API.Services;

public class CleanDatabaseService : CronJobService
{
    private readonly IServiceProvider _serviceProvider;

    public CleanDatabaseService(IScheduleConfig<CleanDatabaseService> config, IServiceProvider serviceProvider)
        : base(config.CronExpression)
    {
        _serviceProvider = serviceProvider;
    }
    
    public override async Task DoWork(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetService<ToDoContext>() ?? throw new Exception("Can't create db context");
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE public.\"ToDoItems\" RESTART IDENTITY;", cancellationToken);
            await context.ToDoItems.AddRangeAsync(new List<ToDoItem>
            {
                new()
                {
                    Name = "Read book",
                    Description = "Read at least 10 books",
                    IsCompleted = false,
                    Priority = 2,
                    CreatedDate = DateTime.UtcNow.AddSeconds(-713822),
                    UpdatedDate = DateTime.UtcNow.AddSeconds(-713822)
                },
                new()
                {
                    Name = "Go to the shop",
                    Description = "Buy: bread, butter, cheese",
                    IsCompleted = false,
                    Priority = 1,
                    CreatedDate = DateTime.UtcNow.AddSeconds(-1412),
                    UpdatedDate = DateTime.UtcNow.AddSeconds(-1293)
                },
                new()
                {
                    Name = "Call to grandpa",
                    Description = null,
                    IsCompleted = true,
                    Priority = 3,
                    CreatedDate = DateTime.UtcNow.AddSeconds(-713222),
                    UpdatedDate = DateTime.UtcNow.AddSeconds(-7222)
                },
                new()
                {
                    Name = "Clean house",
                    Description = null,
                    IsCompleted = false,
                    Priority = 2,
                    CreatedDate = DateTime.UtcNow.AddSeconds(-713622),
                    UpdatedDate = DateTime.UtcNow.AddSeconds(-713622)
                },
                new()
                {
                    Name = "Do homework",
                    Description = "Math, Chem",
                    IsCompleted = true,
                    Priority = 1,
                    CreatedDate = DateTime.UtcNow.AddSeconds(-78374),
                    UpdatedDate = DateTime.UtcNow.AddSeconds(-7834)
                }
            }, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
        }
    }
}
