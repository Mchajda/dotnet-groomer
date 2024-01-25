using dotnet_groomer.Models;
using dotnet_groomer.Models.Visit;
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

        modelBuilder.Entity<VisitProduct>()
            .ToTable("visit_products")
            .HasKey(vp => new { vp.VisitId, vp.ProductId });

        modelBuilder.Entity<VisitProduct>()
            .HasOne(vp => vp.Visit)
            .WithMany(v => v.VisitProducts)
            .HasForeignKey(vp => vp.VisitId);

        modelBuilder.Entity<VisitProduct>()
            .HasOne(vp => vp.Product)
            .WithMany(p => p.VisitProducts)
            .HasForeignKey(vp => vp.ProductId);
    }
}
