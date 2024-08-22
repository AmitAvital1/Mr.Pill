using Microsoft.EntityFrameworkCore;
using Login.Models.LoginService;

namespace Login.Models.DB;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User>? Users { get; set; }
    public DbSet<MedicineCabinet> MedicineCabinets { get; set; }
    public DbSet<MedicineCabinetUsers> MedicineCabinetUsers { get; set; }
    public DbSet<PhoneMessage> PhoneMessages { get; set; }
    public DbSet<CabinetRequest> CabinetRequests{ get; set; }
}