using Microsoft.EntityFrameworkCore;

namespace PN.Models.DB;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<UserMedications> UserMedications { get; set; }
    public DbSet<MedicationRepo> MedicationRepos { get; set; }
    public DbSet<MedicineCabinet> MedicineCabinets { get; set; }
    public DbSet<MedicineCabinetUsers> MedicineCabinetUsers { get; set; }
    public DbSet<Reminder> Reminders { get; set; }

}