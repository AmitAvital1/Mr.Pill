namespace UserServiceApp.Models.ManagerService;

public class ManagerService : IManagerService
{
    // have a method that start to invoke when the procees start to run and check in the db if their is some expired medications
    private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1); 
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private readonly AppDbContext _dbContext;
    private readonly ILogger _logger;

    public ManagerService(AppDbContext dbContext,  ILogger<ManagerService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task StartAsync()
    {
        _logger.LogInformation("StartAsync method initiated. Monitoring for cancellation and expired medications.");

        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            await _semaphoreSlim.WaitAsync(_cancellationTokenSource.Token).ConfigureAwait(false); 

            try
            {
                _logger.LogInformation("Semaphore acquired. Checking for expired medications.");
                await CheckAndDeleteExpiredMedicationsAsync(_cancellationTokenSource.Token);
                _logger.LogInformation("Check for expired medications completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking and deleting expired medications.");
                throw;
            }
            finally
            {
                _logger.LogInformation("Releasing semaphore.");
                _semaphoreSlim.Release(); 
            }

            // Wait 10 seconds with cancellation support
            _logger.LogInformation("Waiting for 10 seconds before the next check.");
            await Task.Delay(10000, _cancellationTokenSource.Token); 
        }

        _logger.LogInformation("Cancellation requested. Exiting StartAsync method.");
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    private async Task CheckAndDeleteExpiredMedicationsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking for expired medications.");

        List<MedicationRepo> expiredMedications = await GetExpiredMedicationsAsync(cancellationToken);

        foreach (var medication in expiredMedications)
        {
            await DeleteMedicationAsync(medication, cancellationToken);
        }
    }

    private async Task<List<MedicationRepo>> GetExpiredMedicationsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Querying the database for expired medications.");

        throw new NotImplementedException();
    }

    private async Task DeleteMedicationAsync(MedicationRepo medication, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}