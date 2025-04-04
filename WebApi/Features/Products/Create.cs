using Ardalis.Result;
using Contracts;
using Mapster;
using WebApi.Abstractions;
using WebApi.Entities;
using Wolverine;

namespace WebApi.Features.Products;

public sealed record CreateProductRequest(string Name, string Description, decimal Price);

public class CreateProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("products", async (
            CreateProductRequest request,
            IMessageBus bus,
            CancellationToken cancellationToken) =>
        {
            var command = request.Adapt<CreateProductCommand>();

            var response = await bus.InvokeAsync<Result<Guid>>(command, cancellationToken);

            return response.IsSuccess
                ? Results.Ok(response.Value)
                : Results.BadRequest();
        }).WithTags(Tags.Products);
    }
}

public record CreateProductCommand(string Name, string Description, decimal Price);

public class CreateProductCommandHandler(IMessageBus bus, IApplicationDbContext dbContext)
{
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(
            Guid.NewGuid(),
            DateTime.UtcNow,
            request.Name,
            request.Description,
            request.Price);

        dbContext.Products.Add(product);

        await dbContext.SaveChangesAsync(cancellationToken);

        var message = new ProductCreated(product.Id, product.Name, product.Description);
        await bus.PublishAsync(message);

        return Result.Success(product.Id);
    }
}