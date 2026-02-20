
namespace WebRabbit;
public record PackageOrderEvent(string ShippingId) :IShipEvent;