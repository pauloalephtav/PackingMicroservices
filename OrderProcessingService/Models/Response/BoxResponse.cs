﻿namespace OrderProcessingService.Models.Response
{
    public class BoxResponse
    {
        public string? BoxId { get; set; }
        public string? Observation { get; set; }
        public List<ProductResponse> Products { get; set; }
    }
}