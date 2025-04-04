using Contracts;
using WebApi.Extensions;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSwaggerGen()
    .AddEndpointsApiExplorer()
    .AddEndpoints()
    .AddDatabase(builder.Configuration);

builder.Host.UseWolverine(options =>
{
    options.UseRabbitMq(new Uri("amqp://rabbitmq:5672"))
        .AutoProvision();

    options.PublishMessage<ProductCreated>().ToRabbitQueue("product-queue");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();