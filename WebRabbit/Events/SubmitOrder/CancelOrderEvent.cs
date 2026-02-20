using MassTransit;

namespace WebRabbit;
public record SubmitOrderEvent(string OrderId):IOrderEvent;