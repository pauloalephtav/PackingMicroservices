namespace PackingAlgorithmService.Models.Response
{
    public class PackingResponse
    {
        public string OrderId { get; set; }
        public List<BoxResponse> Boxes { get; set; }
    }
}