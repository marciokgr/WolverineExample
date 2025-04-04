using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebApi.Abstractions;
using WebApi.Entities;

namespace WebApi.Database;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

    public DbSet<Product> Products { get; set; }
}