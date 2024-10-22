using PackingAlgorithmService.Models;
using PackingAlgorithmService.Models.Response;

namespace PackingAlgorithmService.Services
{
    public interface IPackingService
    {
        Task<List<BoxResponse>> PackOrder(Order order);
    }
}
