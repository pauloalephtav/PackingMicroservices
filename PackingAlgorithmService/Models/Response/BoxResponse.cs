namespace PackingAlgorithmService.Models.Response
{
    public class BoxResponse
    {
        public BoxResponse()
        {
            BoxId = null;
            Observation = null;
            Products = [];
        }

        public BoxResponse(string boxId, List<ProductResponse> product)
        {
            BoxId = boxId;
            Products = product;
            Observation = null;

        }
        public BoxResponse(List<ProductResponse> product, string observation)
        {
            BoxId = null;
            Products = product;
            Observation = observation;

        }

        public BoxResponse(string boxId, List<ProductResponse> product, string observation)
        {
            BoxId = boxId;
            Products = product;
            Observation = observation;

        }
        public string? BoxId { get; set; }
        public string? Observation { get; set; }
        public List<ProductResponse> Products { get; set; }
    }
}