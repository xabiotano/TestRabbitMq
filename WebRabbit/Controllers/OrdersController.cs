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

        [HttpGet]
        [Route("schedule/createorder")]
        public async Task<Guid> ScheduleCreateOrder()
        {
            var orderEvent = new SubmitOrderEvent($"CREATED {Guid.NewGuid().ToString()}");

            var message = await messageScheduler.SchedulePublish(
                DateTime.UtcNow + TimeSpan.FromSeconds(20),
                orderEvent);

            return message.TokenId;
        }

        [HttpGet]
        [Route("schedule/shiporder")]
        public async Task<Guid> ScheduleShipOrder()
        {
            var orderEvent = new ShipOrderEvent($"SHIPPED {Guid.NewGuid().ToString()}");

            var message = await messageScheduler.SchedulePublish(
                DateTime.UtcNow + TimeSpan.FromSeconds(20),
                orderEvent);

            return message.TokenId;
        }

        [HttpGet]
        [Route("cancel-schedule/{tokenId}")]
        public async Task CancelSchedule(string tokenId)
        {
            var orderEvent = new ShipOrderEvent($"SHIPPED {Guid.NewGuid().ToString()}");
            await messageScheduler.CancelScheduledPublish<ShipOrderEvent>(Guid.Parse(tokenId)); // this will never work :)

        }


        [HttpGet]
        [Route("schedule/packageorder")]
        public async Task<Guid> SchedulePackageOrder()
        {
            var packageEvent = new PackageOrderEvent($"PACKAGED {Guid.NewGuid().ToString()}");

            var message = await messageScheduler.SchedulePublish(
                DateTime.UtcNow + TimeSpan.FromSeconds(20),
                packageEvent);

            return message.TokenId;
        }
    }
}
