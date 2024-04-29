
using Microsoft.EntityFrameworkCore;
using UserServiceApp.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User>? Users { get; set; }
    public DbSet<Medication> Medications { get; set; }
    public DbSet<MedicationRepo> MedicationRepos { get; set; }
    public DbSet<House> Houses { get; set; }
    public DbSet<UserHouse> UserHouses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserHouse>()
            .HasKey(userHouseTable => new { userHouseTable.UserId, userHouseTable.HouseId });

        modelBuilder.Entity<UserHouse>()
            .HasOne(userHouseTable => userHouseTable.User)
            .WithMany(user => user.UserHouses)
            .HasForeignKey(userHouseTable => userHouseTable.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<UserHouse>()
            .HasOne(userHouseTable => userHouseTable.House)
            .WithMany(house => house.UserHouses)
            .HasForeignKey(userHouseTable => userHouseTable.HouseId)
            .OnDelete(DeleteBehavior.NoAction);


        modelBuilder.Entity<Medication>()
            .HasOne(medication => medication.MedicationRepo)
            .WithMany(medicationRepo => medicationRepo.Medications)
            .HasForeignKey(medication => medication.MedicationRepoId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Medication>()
            .HasOne(medication => medication.User)
            .WithMany(user => user.Medications)
            .HasForeignKey(medication => medication.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<User>()
            .HasIndex(user => user.PhoneNumber)
            .IsUnique();

    }
}