using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using WebApi.Abstractions;
using Wolverine;

namespace WebApi.Features.Products;

public sealed class DeleteProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("products/{id:guid}", async (IMessageBus bus, Guid id, CancellationToken cancellationToken) =>
        {
            var command = new DeleteProductCommand(id);

            var response = await bus.InvokeAsync<Result>(command, cancellationToken);

            return response.IsSuccess
                ? Results.NoContent()
                : Results.NotFound();
        }).WithTags(Tags.Products);
    }
}

public record DeleteProductCommand(Guid Id);

public class DeleteProductCommandHandler(IApplicationDbContext dbContext)
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (product is null)
        {
            return Result.NotFound();
        }

        dbContext.Products.Remove(product);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}