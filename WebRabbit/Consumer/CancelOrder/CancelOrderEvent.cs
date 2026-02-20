
namespace WebRabbit;
public record CancelOrderEvent(string OrderId):IOrderEvent;