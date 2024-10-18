using OrderProcessingService.Models;
using OrderProcessingService.Models.Response;

namespace OrderProcessingService.Infra.Interfaces
{
    public interface IPackingAlgorithmService
    {
        Task<List<PackingResponse>> CallPackingAlgorithmService(List<Order> orders);
    }
}