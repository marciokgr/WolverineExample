using Microsoft.EntityFrameworkCore;
using WebApi.Abstractions;
using Wolverine;

namespace WebApi.Features.Products;

public sealed class GetAllProductsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("products/", async (IMessageBus bus, CancellationToken cancellationToken) =>
        {
            var query = new GetAllProductsQuery();

            var response = await bus.InvokeAsync<IEnumerable<ProductResponse?>>(query, cancellationToken);

            return Results.Ok(response);
        }).WithTags(Tags.Products);
    }
}

public sealed record GetAllProductsQuery;

public sealed class GetAllProductsQueryHandler(IApplicationDbContext dbContext)
{
    public async Task<IEnumerable<ProductResponse?>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken) =>
        await dbContext.Products
            .Select(p => new ProductResponse(p.Id, p.Name, p.Description, p.Price))
            .ToListAsync(cancellationToken);
}