using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

namespace WebApi.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}