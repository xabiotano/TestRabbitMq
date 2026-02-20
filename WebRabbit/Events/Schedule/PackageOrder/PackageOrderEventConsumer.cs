using MassTransit;

namespace WebRabbit;
public class PackageOrderEventConsumer(ILogger<PackageOrderEventConsumer> logger) : IConsumer<PackageOrderEvent>
{

    public async Task Consume(ConsumeContext<PackageOrderEvent> context)
    {
        logger.LogInformation($"Consumed {nameof(PackageOrderEventConsumer)}");
    }
}