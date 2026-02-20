using MassTransit;
using MassTransit.Scheduling;
using MassTransit.Transports;
using Microsoft.AspNetCore.Mvc;

namespace WebRabbit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(
        IPublishEndpoint publishEndpoint, 
        ILogger<OrdersController> logger,
        IMessageScheduler messageScheduler)
        : ControllerBase
    {


        [HttpGet]
        [Route("SubmitOrderEvent")]
        public async Task SubmitOrderEvent()
        {
            var orderEvent = new SubmitOrderEvent($"CREATED {Guid.NewGuid().ToString()}");
            await publishEndpoint.Publish(orderEvent, context =>
            {
                context.SetRoutingKey(orderEvent.OrderId);
            });
        }
        
        [HttpGet]
        [Route("CancelOrderEvent")]
        public async Task CancelOrderEvent()
        {
            var orderEvent = new CancelOrderEvent($"CANCELED {Guid.NewGuid().ToString()}");
            await publishEndpoint.Publish(orderEvent, context =>
            {
                context.SetRoutingKey(orderEvent.OrderId);
            });
        }
        
        [HttpGet]
        [Route("ShipOrderEvent")]
        public async Task ShipOrderEvent()
        {
            var orderEvent = new ShipOrderEvent($"SHIPPED {Guid.NewGuid().ToString()}");
            await publishEndpoint.Publish(orderEvent);
        }

        [HttpGet]
        [Route("PackageOrderEvent")]
        public async Task PackageOrderEvent()
        {
            var orderEvent = new PackageOrderEvent($"PACKAGED {Guid.NewGuid().ToString()}");
            await publishEndpoint.Publish(orderEvent);
        }
    }
}
