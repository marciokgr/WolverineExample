namespace WebApi.Features.Products;

public sealed record ProductResponse(Guid Id, string Name, string Description, decimal Price);