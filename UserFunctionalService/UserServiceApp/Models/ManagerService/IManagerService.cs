namespace UserServiceApp.Models.ManagerService;

public interface IManagerService
{
    Task StartAsync();
     void Stop();
}