using MassTransit;
using SharedContracts;

var builder = WebApplication.CreateBuilder(args);

// Habilita a documentação visual (Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura o MassTransit como Produtor no RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Endpoint Minimal API que recebe o POST e publica na fila
app.MapPost("/api/orders", async (CreateOrderRequest request, IPublishEndpoint publishEndpoint) =>
{
    var orderEvent = new OrderCreatedEvent
    {
        CustomerEmail = request.CustomerEmail,
        TotalAmount = request.TotalAmount
    };

    // Publica o evento de forma assíncrona para o RabbitMQ
    await publishEndpoint.Publish(orderEvent);

    // Retorna HTTP 202 (Accepted) imediatamente com o ID gerado
    return Results.Accepted($"/api/orders/{orderEvent.OrderId}", new
    {
        orderEvent.OrderId,
        Status = "Processing in background",
        orderEvent.CreatedAt
    });
});

app.Run();

// DTO de entrada da requisição HTTP
public record CreateOrderRequest(string CustomerEmail, decimal TotalAmount);