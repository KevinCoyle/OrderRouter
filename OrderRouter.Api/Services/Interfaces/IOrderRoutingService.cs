using OrderRouter.Api.Contracts.Requests;
using OrderRouter.Api.Contracts.Responses;

namespace OrderRouter.Api.Services.Interfaces;

public interface IOrderRoutingService
{
    public OrderRoutingResponse RouteOrders(OrderRoutingRequest orderRoutingRequest);
}