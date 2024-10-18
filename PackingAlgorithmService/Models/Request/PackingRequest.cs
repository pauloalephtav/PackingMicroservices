namespace PackingAlgorithmService.Models.Request
{
    public class PackingRequest
    {
        public string SecretKey { get; set; }
        public List<Order> Orders { get; set; }
    }
}
