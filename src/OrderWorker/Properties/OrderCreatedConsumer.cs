using MassTransit;
using SharedContracts;

namespace OrderWorker;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "[WORKER] Recebido pedido {OrderId} | Cliente: {Email} | Valor: R$ {Total}",
            message.OrderId, message.CustomerEmail, message.TotalAmount);

        // Simula o tempo de processamento (ex: gateway de pagamento ou geração de nota)
        await Task.Delay(1000);

        _logger.LogInformation(
            "[WORKER] Pedido {OrderId} processado e faturado com sucesso!",
            message.OrderId);
    }
}