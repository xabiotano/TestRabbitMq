using MassTransit;

namespace WebRabbit;
public class ShipOrderEventConsumer(ILogger<ShipOrderEventConsumer> logger) : IConsumer<ShipOrderEvent>
{

    public async Task Consume(ConsumeContext<ShipOrderEvent> context)
    {
        logger.LogInformation($"Consumed {nameof(ShipOrderEventConsumer)}");
    }
}