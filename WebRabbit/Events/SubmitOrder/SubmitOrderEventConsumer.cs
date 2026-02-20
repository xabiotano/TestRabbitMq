using MassTransit;

namespace WebRabbit;
public class SubmitOrderEventConsumer (ILogger<SubmitOrderEventConsumer> logger) :
    IConsumer<SubmitOrderEvent>
{
    public async Task Consume(ConsumeContext<SubmitOrderEvent> context)
    {
        logger.LogInformation($"Consumed {nameof(SubmitOrderEventConsumer)}");
    }
}