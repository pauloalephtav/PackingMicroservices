using APIGateway.Models;
using APIGateway.Models.Request;

namespace APIGateway.Infra.Interfaces
{
    public interface IOrdersService
    {
        Task<OrderResponse> CallOrderProcessingService(List<Order> orders);
    }
}