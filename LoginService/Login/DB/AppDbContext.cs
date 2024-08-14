using Microsoft.EntityFrameworkCore;
using Login.Models.LoginService;

namespace Login.Models.DB;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User>? Users { get; set; }
    public DbSet<MedicineCabinets> MedicineCabinets { get; set; }
    public DbSet<MedicineCabinetUsers> MedicineCabinetUsers { get; set; }
}