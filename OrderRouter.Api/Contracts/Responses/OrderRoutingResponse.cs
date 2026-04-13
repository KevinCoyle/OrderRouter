using System.Text.Json.Serialization;

namespace OrderRouter.Api.Contracts.Responses;

[Serializable]
public class OrderRoutingResponse
{
    [JsonPropertyName("feasible")] public bool IsFeasible => (Errors?.Count ?? 0) == 0;

    [JsonPropertyName("routing")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<OrderRoutingDetails>? Routing { get; set; } = new();

    [JsonPropertyName("errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Errors { get; set; } = new();
}