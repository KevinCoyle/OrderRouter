using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CsvHelper.Configuration.Attributes;

namespace OrderRouter.Api.Models;

public class Product
{
    [Key]
    [Name("product_code")]
    [JsonPropertyName("product_code")]
    public string Id { get; set; }
    
    [Name("product_name")]
    [JsonPropertyName("product_name")]
    public string Name { get; set; }
    
    [Name("category")]
    [TypeConverter(typeof(ProductCategoryConverter))]
    [JsonPropertyName("category")]
    public ProductCategory Category { get; set; }
}