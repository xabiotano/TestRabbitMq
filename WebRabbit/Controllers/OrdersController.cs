using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Mvc;

namespace WebRabbit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(IPublishEndpoint publishEndpoint, ILogger<OrdersController> logger)
        : ControllerBase
    {


        [HttpGet]
        [Route("create")]
        public async Task PublicarOrderCreated()
        {
            var orderEvent = new SubmitOrderEvent($"CREATED {Guid.NewGuid().ToString()}");
            await publishEndpoint.Publish(orderEvent, context =>
            {
                context.SetRoutingKey(orderEvent.OrderId);
            });
        }
        
        [HttpGet]
        [Route("cancel")]
        public async Task PublicarOrderCanceled()
        {
            var orderEvent = new CancelOrderEvent($"CANCELED {Guid.NewGuid().ToString()}");
            await publishEndpoint.Publish(orderEvent, context =>
            {
                context.SetRoutingKey(orderEvent.OrderId);
            });
        }
    }
}
