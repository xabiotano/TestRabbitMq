using MassTransit;
using WebRabbit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMassTransit(x =>
{
    x.AddDelayedMessageScheduler();


    x.AddConsumer<SubmitOrderEventConsumer>();
    x.AddConsumer<CancelOrderEventConsumer>();
    x.AddConsumer<PackageOrderEventConsumer>();
    x.AddConsumer<ShipOrderEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("host.docker.internal", "/", h =>
        {
            h.Username("admin");
            h.Password("123456");
        });
        cfg.UseDelayedMessageScheduler();

        cfg.Message<IOrderEvent>(x => x.SetEntityName("order-events-hash"));
        cfg.Publish<IOrderEvent>(x => x.ExchangeType = "x-consistent-hash");

        cfg.Message<SubmitOrderEvent>(x => x.SetEntityName("order-events-hash"));
        cfg.Publish<SubmitOrderEvent>(x =>
        {
            x.Exclude = true;
            x.ExchangeType = "x-consistent-hash";
        });

        cfg.Message<CancelOrderEvent>(x => x.SetEntityName("order-events-hash"));
        cfg.Publish<CancelOrderEvent>(x =>
        {
            x.Exclude = true;
            x.ExchangeType = "x-consistent-hash";
        });

        //Shipping events
        cfg.Message<IShipEvent>(x => x.SetEntityName("ship-events-ex"));
       
        // cfg.Publish<IOrderEvent>(x => x.ExchangeType = "x-consistent-hash"); no need to define it as is default
        cfg.Message<ShipOrderEvent>(x => x.SetEntityName("ship-events-ex"));
        cfg.Publish<ShipOrderEvent>(x =>
        {
            x.Exclude = true; //no need to define exchangetype as is default
        });
        cfg.Message<PackageOrderEvent>(x => x.SetEntityName("ship-events-ex"));
        cfg.Publish<PackageOrderEvent>(x =>
        {
            x.Exclude = true; //no need to define exchangetype as is default
        });

        for (int i = 0; i < 4; i++) //4 Pods se esperan
        {
            cfg.ReceiveEndpoint($"order-events-{i}", e =>
            {
                e.ConfigureConsumeTopology = false;

                e.ConfigureConsumer<SubmitOrderEventConsumer>(context);
                e.ConfigureConsumer<CancelOrderEventConsumer>(context);

                e.ConcurrentMessageLimit = 1; // solo un mensaje puede ser consumido concurrentemente
                e.PrefetchCount = 10; // cuando mensajes pre-almaceno en el Consumer para ser consumidos
                e.SingleActiveConsumer = true;

                e.Bind("order-events-hash", s =>
                {
                    s.ExchangeType = "x-consistent-hash";
                    s.RoutingKey = "1";                   
                });
            });
        }


        cfg.ReceiveEndpoint($"ship-events", e =>
        {
            e.ConfigureConsumeTopology = false;

            e.ConfigureConsumer<ShipOrderEventConsumer>(context);
            e.ConfigureConsumer<PackageOrderEventConsumer>(context);

            e.Bind("ship-events-ex");
        });
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
