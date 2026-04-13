using System.Text.Json.Serialization;

namespace OrderRouter.Api.Contracts.Responses;

[Serializable]
public class OrderRoutingDetails
{
    [JsonPropertyName("supplier_id")]
    public string SupplierId { get; set; }
    
    [JsonPropertyName("supplier_name")]
    public string SupplierName { get; set; }
    
    [JsonPropertyName("items")]
    public List<OrderRoutingItem> Items { get; set; }
}