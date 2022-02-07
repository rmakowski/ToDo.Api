using Cronos;

namespace ToDo.API.Services;

public class CronJobService : IHostedService, IDisposable
{
    private System.Timers.Timer? _timer;
    private readonly CronExpression _expression;

    protected CronJobService(string cronExpression)
    {
        _expression = CronExpression.Parse(cronExpression);
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        await ScheduleJob(cancellationToken);
    }

    protected virtual async Task ScheduleJob(CancellationToken cancellationToken)
    {
        var next = _expression.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Utc);
        if (next.HasValue)
        {
            var delay = next.Value - DateTimeOffset.Now;
            if (delay.TotalMilliseconds <= 0)
                await ScheduleJob(cancellationToken);
            _timer = new System.Timers.Timer(delay.TotalMilliseconds);
            _timer.Elapsed += async (_, _) =>
            {
                _timer.Dispose();
                _timer = null;
                if (!cancellationToken.IsCancellationRequested)
                    await DoWork(cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                    await ScheduleJob(cancellationToken);
            };
            _timer.Start();
        }
        await Task.CompletedTask;
    }

    public virtual async Task DoWork(CancellationToken cancellationToken)
    {
        await Task.Delay(5000, cancellationToken);
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Stop();
        await Task.CompletedTask;
    }

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        _timer?.Dispose();
    }
}
