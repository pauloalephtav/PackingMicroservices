namespace OrderProcessingService.Models.Request
{
    public class OrderRequest
    {
        public string SecretKey { get; set; }
        public List<Order> Orders { get; set; }
    }
}
