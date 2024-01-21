using dotnet_groomer.Models;
using Microsoft.EntityFrameworkCore;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    { }

    public DbSet<User> Users { get; set; }
    public DbSet<Visit> Visits { get; set; }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the User entity to map to the "users" table
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<Visit>().ToTable("visits");
        modelBuilder.Entity<Product>().ToTable("products");
    }
}
