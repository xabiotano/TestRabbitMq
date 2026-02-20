using MassTransit;

namespace WebRabbit;
public class CancelOrderEventConsumer (ILogger<CancelOrderEventConsumer> logger) :
    IConsumer<CancelOrderEvent>
{

    public async Task Consume(ConsumeContext<CancelOrderEvent> context)
    {
        logger.LogInformation($"Consumed {nameof(CancelOrderEventConsumer)}");
    }
}