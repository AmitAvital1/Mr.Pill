
using Microsoft.EntityFrameworkCore;
using UserServiceApp.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<UserMedications> UserMedications { get; set; }
    public DbSet<MedicationRepo> MedicationRepos { get; set; }
    public DbSet<MedicineCabinet> MedicineCabinets { get; set; }
    public DbSet<MedicineCabinetUsers> MedicineCabinetUsers { get; set; }
    public DbSet<CabinetRequest> CabinetRequests{ get; set; }
    public DbSet<PhoneMessage> PhoneMessages { get; set; }
    public DbSet<Reminder> Reminders { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MedicineCabinetUsers>()
            .HasKey(MedicineCabinetUsersTable => new { MedicineCabinetUsersTable.UserId, MedicineCabinetUsersTable.MedicineCabinetId });

        modelBuilder.Entity<MedicineCabinetUsers>()
            .Property(mcu => mcu.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<MedicineCabinetUsers>()
            .HasOne(MedicineCabinetUsersTable => MedicineCabinetUsersTable.User)
            .WithMany(user => user.MedicineCabinetUsersList)
            .HasForeignKey(MedicineCabinetUsersTable => MedicineCabinetUsersTable.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<MedicineCabinetUsers>()
            .HasOne(MedicineCabinetUsersTable => MedicineCabinetUsersTable.MedicineCabinet)
            .WithMany(MedicineCabinet => MedicineCabinet.MedicineCabinetUsers)
            .HasForeignKey(MedicineCabinetUsersTable => MedicineCabinetUsersTable.MedicineCabinetId)
            .OnDelete(DeleteBehavior.NoAction);


        modelBuilder.Entity<UserMedications>()
            .HasOne(medication => medication.MedicationRepo)
            .WithMany(medicationRepo => medicationRepo.Medications)
            .HasForeignKey(medication => medication.MedicationRepoId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<UserMedications>()
            .HasOne(medication => medication.MedicineCabinet)
            .WithMany(MedicineCabinet => MedicineCabinet.Medications)
            .HasForeignKey(medication => medication.MedicineCabinetId)
            .OnDelete(DeleteBehavior.Cascade);// change from NoAction to Cascade

        modelBuilder.Entity<User>()
            .HasIndex(user => user.PhoneNumber)
            .IsUnique();

        modelBuilder.Entity<MedicineCabinet>()
            .HasOne(MedicineCabinet => MedicineCabinet.Creator)
            .WithMany()
            .HasForeignKey(MedicineCabinet => MedicineCabinet.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reminder>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reminders)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Reminder>()
            .HasOne(r => r.UserMedication)
            .WithMany(um => um.Reminders)
            .HasForeignKey(r => r.UserMedicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}