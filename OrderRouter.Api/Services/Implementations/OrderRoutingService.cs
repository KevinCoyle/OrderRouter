using OrderRouter.Api.Contracts.Requests;
using OrderRouter.Api.Contracts.Responses;
using OrderRouter.Api.Database;
using OrderRouter.Api.Models;
using OrderRouter.Api.Services.Interfaces;

namespace OrderRouter.Api.Services.Implementations;

public class OrderRoutingService  : IOrderRoutingService
{
    private readonly OrderRouterDbContext _context;
    
    public OrderRoutingService(OrderRouterDbContext dbContext)
    {
        _context = dbContext;
    }
    public OrderRoutingResponse RouteOrders(OrderRoutingRequest request)
    {
        var response = new OrderRoutingResponse();

        if ((request?.Items?.Count() ?? 0) == 0)
        {
            response.Errors.Add("Order must include at least one line item.");
        }

        if (!IsInRange(request.CustomerZipCode, 10000, 99999))
        {
            response.Errors.Add("Order must include a valid customer_zip.");
        }

        if (!response.IsFeasible)
        {
            response.Routing = null;
            return response;
        }

        var allSuppliers = _context.Suppliers.ToList();
        
        var orderSuppliers = allSuppliers
            .Where( s 
                => ZipCodesMatch(s.ZipCodes, request.CustomerZipCode) 
                    || (request.IsMailOrder && s.CanMailOrder)
            )
            .OrderByDescending(s => s.CustomerSatisfactionScore ?? decimal.MinValue)
            .ToList();
        
        var orderedProducts = _context.Products
            .Where(p 
                => request.Items.Select(o 
                    => o.ProductCode
                ).Contains(p.Id)
            )
            .ToList();
        var productCategoriesNeeded = orderedProducts
            .Select(p => p.Category)
            .Distinct()
            .ToList();

        var productCategoriesFound = new List<ProductCategory>();
        while (productCategoriesNeeded.Any() 
               && (!response.Errors?.Any() ?? true))
        {
            var bestMatchSupplier =
                orderSuppliers.MaxBy(s 
                    => s.ProductCategories.Intersect(productCategoriesNeeded).Count()
                );
            
            var matchedProductCategories = bestMatchSupplier.ProductCategories.Intersect(productCategoriesNeeded).ToList();
            
            // No suppliers found with needed ProductCategories
            if (matchedProductCategories.Count <= 0)
            {
                response.Errors.Add($"No combination of Suppliers can fulfill this Order.");
                break;
            }
            
            orderSuppliers.ToList().Remove(bestMatchSupplier);

            productCategoriesFound.AddRange(matchedProductCategories);
            
            productCategoriesNeeded.RemoveAll(p 
                => matchedProductCategories.Contains(p)
            );
            
            var matchedProducts = orderedProducts.Where(p => productCategoriesFound.Contains(p.Category));
            response.Routing.Add(new OrderRoutingDetails()
            {
                SupplierId =  bestMatchSupplier.Id,
                SupplierName = bestMatchSupplier.Name,
                Items = ConvertProductsToOrderRoutingItems(matchedProducts, request, (bestMatchSupplier.ZipCodes.Contains(request.CustomerZipCode) ? "local" : "mail"))
            });
        }

        if (response.IsFeasible)
        {
            response.Errors = null;
            return response;
        }

        response.Routing = null;
        return response;
    }

    private bool ZipCodesMatch(List<ZipCode> zipCodes, int customerZipCode)
    {
        foreach (var zipCode in zipCodes)
        {
            if (zipCode.SingleValue == customerZipCode
                || (zipCode.RangeValue.HasValue 
                    && IsInRange(customerZipCode, zipCode.RangeValue.Value.Min, zipCode.RangeValue.Value.Max))
            )
            {
                return true;
            }
        }
        return false;
    }

    private List<OrderRoutingItem> ConvertProductsToOrderRoutingItems(IEnumerable<Product> matchedProducts, OrderRoutingRequest request, string fullFillmentType)
    {
        var result = new List<OrderRoutingItem>();

        foreach (var product in matchedProducts)
        {
            result.Add(new OrderRoutingItem()
            {
                ProductCode = product.Id,
                Quantity = request.Items.FirstOrDefault(p => p.ProductCode == product.Id)?.Quantity ?? 0,
                Category = product.Category.ToString(),
                FulfillmentMode = fullFillmentType
            });
        }
        
        return result;
    }
    
    private static bool IsInRange(int value, int min, int max)
        => (uint)(value - min) <= (uint)(max - min);
}