using System.Text.Json.Serialization;

namespace OrderRouter.Api.Contracts.Responses;

[Serializable]
public class OrderRoutingItem
{
    [JsonPropertyName("product_code")]
    public string ProductCode { get; set; }
    
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
    
    [JsonPropertyName("category")]
    public string Category { get; set; }
    
    [JsonPropertyName("fulfillment_mode")]
    public string FulfillmentMode { get; set; }
}