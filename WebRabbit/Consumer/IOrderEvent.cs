using MassTransit;
namespace WebRabbit;
public interface IOrderEvent
{
    string OrderId { get; }
}