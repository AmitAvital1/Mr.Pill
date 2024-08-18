using PN.Models.DB;

public class ReminderNotificationService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ReminderNotificationService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(SendNotifications, null, TimeSpan.Zero, TimeSpan.FromMinutes(1)); // Check every minute
        return Task.CompletedTask;
    }

    private void SendNotifications(object state)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var now = DateTime.UtcNow;

            var dueReminders = dbContext.Reminders
                .Where(r => r.IsActive && r.ReminderTime <= now)
                .ToList();

            foreach (var reminder in dueReminders)
            {
                // Send notification logic (e.g., email, SMS, push notification)
                // Mark reminder as inactive if it's not recurring
                if (!reminder.IsRecurring)
                {
                    reminder.IsActive = false;
                }
                else
                {
                    // Set next occurrence time for recurring reminders
                    reminder.ReminderTime = reminder.ReminderTime.Add(reminder.RecurrenceInterval);
                }
            }

            dbContext.SaveChanges();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
