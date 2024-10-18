namespace APIGateway.Models.Request
{
    public class BoxResponse
    {
        public string BoxId { get; set; }
        public List<ProductResponse> Products { get; set; }
    }
}