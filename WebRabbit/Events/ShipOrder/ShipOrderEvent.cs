
namespace WebRabbit;
public record ShipOrderEvent(string ShippingId) :IShipEvent;