using System.Text.Json.Serialization;

namespace OrderRouter.Api.Contracts.Requests;

public class OrderRoutingRequest
{
    [JsonPropertyName("order_id")]
    public string Id { get; set; }
    
    [JsonPropertyName("customer_zip")]
    public int CustomerZipCode { get; set; }
    
    [JsonPropertyName("mail_order")]
    public bool IsMailOrder { get; set; }
    
    [JsonPropertyName("items")]
    public IEnumerable<OrderItem> Items { get; set; }
}