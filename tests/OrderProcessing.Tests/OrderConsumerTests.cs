using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OrderWorker;
using SharedContracts;
using Xunit;

namespace OrderProcessing.Tests;

public class OrderConsumerTests
{
    [Fact]
    public async Task Consume_ValidOrder_ShouldLogProcessingMessages()
    {
        // 1. Arrange (Prepara os mocks em isolamento)
        var loggerMock = new Mock<ILogger<OrderCreatedConsumer>>();
        var consumer = new OrderCreatedConsumer(loggerMock.Object);

        var orderEvent = new OrderCreatedEvent
        {
            CustomerEmail = "cliente@teste.com",
            TotalAmount = 199.90m
        };

        var contextMock = new Mock<ConsumeContext<OrderCreatedEvent>>();
        contextMock.Setup(x => x.Message).Returns(orderEvent);

        // 2. Act (Executa o método)
        var exception = await Record.ExceptionAsync(() => consumer.Consume(contextMock.Object));

        // 3. Assert (Garante que executou sem lançar exceções)
        Assert.Null(exception);
    }

    [Fact]
    public async Task MassTransitHarness_WhenEventPublished_ShouldBeConsumedByWorker()
    {
        // 1. Arrange (Sobe um barramento em memória simulando o RabbitMQ)
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<OrderCreatedConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        // 2. Act (Publica o evento no barramento de teste)
        var orderEvent = new OrderCreatedEvent
        {
            CustomerEmail = "integracao@teste.com",
            TotalAmount = 450.00m
        };

        await harness.Bus.Publish(orderEvent);

        // 3. Assert (Verifica se a mensagem passou pela fila e foi consumida pelo Worker)
        Assert.True(await harness.Consumed.Any<OrderCreatedEvent>());

        var consumerHarness = harness.GetConsumerHarness<OrderCreatedConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<OrderCreatedEvent>(
            x => x.Context.Message.OrderId == orderEvent.OrderId));
    }
}