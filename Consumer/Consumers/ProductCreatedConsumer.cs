using Contracts;

namespace Consumer.Consumers;

public class ProductCreatedConsumer
{
    public void Consume(ProductCreated message, ILogger<ProductCreatedConsumer> logger)
    {
        logger.LogInformation("Message received successfully: {Id} {Name} {Description}",
            message.Id,
            message.Name,
            message.Description);
    }
}