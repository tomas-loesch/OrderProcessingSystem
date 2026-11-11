using MassTransit;
using OrderWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    // Registra o consumidor na injeção de dependência
    x.AddConsumer<OrderCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Define a fila onde o Worker vai escutar
        cfg.ReceiveEndpoint("order-created-queue", e =>
        {
            // Se der erro (ex: banco fora do ar), tenta 3 vezes com intervalo de 2 segundos
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(2)));
            
            e.ConfigureConsumer<OrderCreatedConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();