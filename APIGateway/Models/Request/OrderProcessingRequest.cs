namespace APIGateway.Models.Request
{
    public class OrderProcessingRequest
    {
        public string SecretKey { get; set; }
        public List<Order> Orders { get; set; }
    }
}
