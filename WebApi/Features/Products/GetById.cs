using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using WebApi.Abstractions;
using Wolverine;

namespace WebApi.Features.Products;

public sealed class GetProductByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("products/{id:guid}", async (IMessageBus bus, Guid id, CancellationToken cancellationToken) =>
        {
            var query = new GetProductByIdQuery(id);

            var response = await bus.InvokeAsync<Result<ProductResponse>>(query, cancellationToken);

            return response.IsSuccess
                ? Results.Ok(response.Value)
                : Results.NotFound();
        }).WithTags(Tags.Products);
    }
}

public record GetProductByIdQuery(Guid Id);

// Method injection, similar to the update method, is also an option.
public class GetProductByIdQueryHandler(IApplicationDbContext dbContext)
{
    public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var response = await dbContext.Products
            .Where(x => x.Id == request.Id)
            .Select(x => new ProductResponse(
                x.Id,
                x.Name,
                x.Description,
                x.Price))
            .FirstOrDefaultAsync(cancellationToken);

        return response is null
            ? Result.NotFound()
            : Result.Success(response);
    }
}