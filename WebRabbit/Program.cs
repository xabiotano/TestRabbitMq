using MassTransit;
using WebRabbit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SubmitOrderEventConsumer>();
    x.AddConsumer<CancelOrderEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("host.docker.internal", "/", h =>
        {
            h.Username("admin");
            h.Password("123456");
        });
        
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
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
