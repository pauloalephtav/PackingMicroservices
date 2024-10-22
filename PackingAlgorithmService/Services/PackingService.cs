using PackingAlgorithmService.Infra.Interfaces;
using PackingAlgorithmService.Models;
using PackingAlgorithmService.Models.Response;

namespace PackingAlgorithmService.Services
{
    public class PackingService(IBoxManagementService boxManagementService,
        ILogger<PackingService> logger) : IPackingService
    {
        private readonly IBoxManagementService _boxManagementService = boxManagementService;
        private readonly ILogger<PackingService> _logger = logger;

        public async Task<List<BoxResponse>> PackOrder(Order order)
        {
            var availableBoxes = await _boxManagementService.GetAvailableBoxes();
            var packedBoxes = new List<BoxResponse>();

            availableBoxes = availableBoxes.OrderBy(b => b.Height * b.Width * b.Length).ToList();

            var productsToPack = new List<Product>(order.Products);

            while (productsToPack.Any())
            {
                var boxResponse = FindBestBoxForProducts(productsToPack, availableBoxes);

                packedBoxes.Add(boxResponse);

                foreach (var product in boxResponse.Products)
                {
                    productsToPack.RemoveAll(p => p.ProductId == product.ProductId);
                }
            }

            _logger.LogInformation($"Order {order.OrderId} packed successfully.", nameof(PackingService));
            return packedBoxes;
        }

        private static BoxResponse FindBestBoxForProducts(List<Product> products, List<Box> availableBoxes)
        {
            foreach (var box in availableBoxes)
            {
                var boxVolume = box.Height * box.Width * box.Length;
                var productsVolume = products.Sum(p => p.Dimension.Height * p.Dimension.Width * p.Dimension.Length);

                if (productsVolume <= boxVolume)
                {
                    var boxDimensionFits = true;
                    foreach (var product in products)
                    {
                        if (box.Height < product.Dimension.Height ||
                            box.Width < product.Dimension.Width ||
                            box.Length < product.Dimension.Length)
                        {
                            boxDimensionFits = false;
                            break;
                        }
                    }

                    if (boxDimensionFits)
                    {
                        var productList = products.Select(p => new ProductResponse { ProductId = p.ProductId }).ToList();
                        return new BoxResponse(box.BoxId, productList);
                    }
                }
            }
            var remainingProducts = products.Select(p => new ProductResponse { ProductId = p.ProductId }).ToList();
            return new BoxResponse(remainingProducts, "Produto não cabe em nenhuma caixa disponível.");
        }
    }
}
