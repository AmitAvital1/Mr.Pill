using Microsoft.EntityFrameworkCore;
using PN.Models.DB;

public class ReminderNotificationService : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private static readonly object _lock = new object();
    private readonly AppDbContext _dbContext;

    public ReminderNotificationService(IServiceScopeFactory serviceScopeFactory, AppDbContext appDbContext)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _dbContext = appDbContext;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(SendNotifications!, null, TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(1)); // Check every minute
        return Task.CompletedTask;
    }

    private void SendNotifications(object state)
    {
        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var now = DateTime.UtcNow.AddHours(3);

                var dueReminders = _dbContext.Reminders
                .Where(r => r.IsActive && r.ReminderTime <= now)
                .ToList();

                foreach (var reminder in dueReminders)
                {
                    var userMedication = _dbContext.UserMedications
                    .Where(um => um.Id == reminder.UserMedicationId)
                    .FirstOrDefault();

                    var medicationRepo = _dbContext.MedicationRepos
                    .Where(um => um.Id == userMedication.MedicationRepoId)
                    .FirstOrDefault();

                    var user = _dbContext.Users
                    .Where(um => um.UserId == reminder.UserId)
                    .FirstOrDefault();

                    Console.WriteLine(user.FirstName + " Need take a pill named: " + medicationRepo.DrugEnglishName);

                    if (!reminder.IsRecurring)
                    {

                        reminder.IsActive = false;
                    }
                    else
                    {
                        reminder.ReminderTime = reminder.ReminderTime.Add(reminder.RecurrenceInterval);
                    }
                }

                _dbContext.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while sending notifications:");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                Console.WriteLine("Inner Exception:");
                Console.WriteLine($"Message: {ex.InnerException.Message}");
                Console.WriteLine($"Stack Trace: {ex.InnerException.StackTrace}");
            }
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
