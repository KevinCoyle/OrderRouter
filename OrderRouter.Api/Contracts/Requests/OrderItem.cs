using System.Text.Json.Serialization;

namespace OrderRouter.Api.Contracts.Requests;

public class OrderItem
{
    [JsonPropertyName("product_code")]
    public string ProductCode { get; set; }
    
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}