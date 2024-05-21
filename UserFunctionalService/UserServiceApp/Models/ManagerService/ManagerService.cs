namespace UserServiceApp.Models.ManagerService;

public class ManagerService : IManagerService
{
    // have a method that start to invoke when the procees start to run and check in the db if their is some expired medications
    private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1); 
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public async Task StartAsync()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            await _semaphoreSlim.WaitAsync(_cancellationTokenSource.Token).ConfigureAwait(false); 

            try
            {
                await CheckAndDeleteExpiredMedicationsAsync(_cancellationTokenSource.Token);
            }
            finally
            {
                _semaphoreSlim.Release(); 
            }

            // Wait 10 seconds with cancellation support
            await Task.Delay(10000, _cancellationTokenSource.Token); 
        }
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    private async Task CheckAndDeleteExpiredMedicationsAsync(CancellationToken cancellationToken)
    {
        List<MedicationRepo> expiredMedications = await GetExpiredMedicationsAsync(cancellationToken);

        foreach (var medication in expiredMedications)
        {
            await DeleteMedicationAsync(medication, cancellationToken);
        }
    }

    private async Task<List<MedicationRepo>> GetExpiredMedicationsAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task DeleteMedicationAsync(MedicationRepo medication, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}