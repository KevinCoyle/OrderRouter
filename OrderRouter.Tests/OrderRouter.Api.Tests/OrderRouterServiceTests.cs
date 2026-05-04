using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using OrderRouter.Api.Contracts.Requests;
using OrderRouter.Api.Contracts.Responses;
using OrderRouter.Tests.OrderRouter.Api.Tests.Shared;

namespace OrderRouter.Tests.OrderRouter.Api.Tests;

public class OrderRouterServiceTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    public OrderRouterServiceTests(CustomWebApplicationFactory factory) => _client = factory.CreateClient();

    private readonly List<OrderRoutingRequest> _requests = JsonSerializer.Deserialize<List<OrderRoutingRequest>>("[{\"order_id\": \"ORD-001\",\"customer_zip\": \"10015\",\"mail_order\": false,\"items\": [{\"product_code\": \"WC-STD-001\",\"quantity\": 1},{\"product_code\": \"OX-PORT-024\",\"quantity\": 1}],\"priority\": \"standard\",\"notes\": \"Simple order - wheelchair + oxygen, should have multiple supplier options in NYC\"},{\"order_id\": \"ORD-002\",\"customer_zip\": \"77059\",\"mail_order\": false,\"items\": [{\"product_code\": \"HB-FUL-018\",\"quantity\": 1},{\"product_code\": \"PL-ELEC-043\",\"quantity\": 1},{\"product_code\": \"CM-BED-048\",\"quantity\": 1},{\"product_code\": \"BP-AUTO-077\",\"quantity\": 1}],\"priority\": \"rush\",\"notes\": \"Larger order with multiple categories - tests consolidation logic\"},{\"order_id\": \"ORD-003\",\"customer_zip\": \"02130\",\"mail_order\": true,\"items\": [{\"product_code\": \"CP-STD-031\",\"quantity\": 1},{\"product_code\": \"CP-MSK-FF-035\",\"quantity\": 2},{\"product_code\": \"NB-COMP-039\",\"quantity\": 1}],\"priority\": \"standard\",\"notes\": \"Respiratory-focused order - mail order allowed, should match to respiratory specialists\"}]", new JsonSerializerOptions { NumberHandling = JsonNumberHandling.AllowReadingFromString })!;
    
    [Fact]
    public async Task RouteSingleOrder_ShouldReturnFeasibleRouting()
    {
        var req = _requests[0];
        var response = await _client.PostAsJsonAsync("/", req);
        
        Assert.True(response.IsSuccessStatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<OrderRoutingResponse>();
        Assert.NotNull(result);
        Assert.True(result.IsFeasible);
        Assert.NotNull(result.Routing);
        Assert.NotEmpty(result.Routing);
    }

    [Fact]
    public async Task RouteListOrders_ShouldReturnBadRequestForInfeasible()
    {
        var response = await _client.PostAsJsonAsync("/list", _requests);
        
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<List<OrderRoutingResponse>>();
        Assert.NotNull(result);
        Assert.Equal(_requests.Count, result.Count);
        
        Assert.True(result[0].IsFeasible);
        Assert.False(result[1].IsFeasible);
        Assert.False(result[2].IsFeasible);
        
        Assert.Contains("No combination of Suppliers can fulfill this Order.", result[1].Errors!);
        Assert.Contains("Order must include a valid customer_zip.", result[2].Errors!);
    }
}
