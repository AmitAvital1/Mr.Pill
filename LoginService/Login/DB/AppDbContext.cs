using Microsoft.EntityFrameworkCore;
using Login.Models.LoginService;

namespace Login.Models.DB;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User>? Users { get; set; }
    public DbSet<House> Houses { get; set; }
    public DbSet<UserHouse> UserHouses { get; set; }
}