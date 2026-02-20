using MassTransit;
using MassTransit.Scheduling;
using MassTransit.Transports;
using Microsoft.AspNetCore.Mvc;

namespace WebRabbit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController(
        IPublishEndpoint publishEndpoint, 
        ILogger<ScheduleController> logger,
        IMessageScheduler messageScheduler)
        : ControllerBase
    {
        [HttpGet]
        [Route("schedule/SubmitOrderEvent")]
        public async Task<Guid> ScheduleSubmitOrderEvent()
        {
            var orderEvent = new SubmitOrderEvent($"CREATED {Guid.NewGuid().ToString()}");

            var message = await messageScheduler.SchedulePublish(
                DateTime.UtcNow + TimeSpan.FromSeconds(20),
                orderEvent);

            return message.TokenId;
        }

        [HttpGet]
        [Route("schedule/ShipOrderEvent")]
        public async Task<Guid> ScheduleShipOrderEvent()
        {
            var orderEvent = new ShipOrderEvent($"SHIPPED {Guid.NewGuid().ToString()}");

            var message = await messageScheduler.SchedulePublish(
                DateTime.UtcNow + TimeSpan.FromSeconds(20),
                orderEvent);

            return message.TokenId;
        }

        [HttpGet]
        [Route("schedule/PackageOrderEvent")]
        public async Task<Guid> SchedulePackageOrderEvent()
        {
            var packageEvent = new PackageOrderEvent($"PACKAGED {Guid.NewGuid().ToString()}");

            var message = await messageScheduler.SchedulePublish(
                DateTime.UtcNow + TimeSpan.FromSeconds(20),
                packageEvent);

            return message.TokenId;
        }


        [HttpGet]
        [Route("cancel-schedule/{tokenId}")]
        public async Task CancelSchedule(string tokenId)
        {
            await messageScheduler.CancelScheduledPublish<ShipOrderEvent>(Guid.Parse(tokenId)); // this will never work :)
        }
    
    }
}
