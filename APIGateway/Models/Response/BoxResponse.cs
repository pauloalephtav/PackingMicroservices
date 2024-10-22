using Newtonsoft.Json;

namespace APIGateway.Models.Request
{
    public class BoxResponse
    {
        public string? BoxId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Observation { get; set; }
        public List<ProductResponse> Products { get; set; }
    }
}