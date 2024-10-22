using PackingAlgorithmService.Models;

namespace PackingAlgorithmService.Infra.Interfaces
{
    public interface IBoxManagementService
    {
        Task<List<Box>> GetAvailableBoxes();
    }
}