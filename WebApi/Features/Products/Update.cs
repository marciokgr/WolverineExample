using Ardalis.Result;
using Mapster;
using Microsoft.EntityFrameworkCore;
using WebApi.Abstractions;
using Wolverine;

namespace WebApi.Features.Products;

public sealed record UpdateProductRequest(string Name, string Description, decimal Price);

public sealed class UpdateProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("products/{id:guid}", async(Guid id, UpdateProductRequest request, IMessageBus bus, CancellationToken cancellationToken) =>
        {
            var command = request.Adapt<UpdateProductCommand>() with { Id = id };

            var response = await bus.InvokeAsync<Result>(command, cancellationToken);

            if (response.IsInvalid())
            {
                return Results.BadRequest();
            }

            if (response.IsNotFound())
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        }).WithTags(Tags.Products);
    }
}

public record UpdateProductCommand(Guid Id, string Name, string Description, decimal Price);

public class UpdateProductCommandHandler
{
    public async Task<Result> Handle(
        UpdateProductCommand request,
        IApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (product is null)
        {
            return Result.NotFound();
        }

        product.Update(request.Name, request.Description, request.Price);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}