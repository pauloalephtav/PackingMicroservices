namespace PackingAlgorithmService.Models
{
    public class Order
    {
        public string OrderId { get; set; }
        public List<Product> Products { get; set; }
    }
}
