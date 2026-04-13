using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;

namespace OrderRouter.Api.Models;

public class Supplier
{
    [Key]
    [Name("supplier_id")]
    [JsonPropertyName("supplier_id")]
    public string Id { get; set; }
    
    [Name("suplier_name")]
    [JsonPropertyName("suplier_name")]
    public string Name { get; set; }

    [Name("service_zips")]
    [TypeConverter(typeof(ZipCodesConverter))]
    [Column(TypeName = "jsonb")]
    [JsonPropertyName("service_zips")]
    public List<ZipCode> ZipCodes { get; set; } = new();

    [Name("product_categories")]
    [TypeConverter(typeof(ProductCategoryConverter))]
    [Column(TypeName = "jsonb")]
    [JsonPropertyName("product_categories")]
    public List<ProductCategory> ProductCategories { get; set; } = new();

    [Name("customer_satisfaction_score")]
    [TypeConverter(typeof(CustomerSatisfactionScoreConverter))]
    [JsonPropertyName("customer_satisfaction_score")]
    public decimal? CustomerSatisfactionScore { get; set; }
    
    [Name("can_mail_order?")]
    [BooleanTrueValues("yes","y","true","t")]
    [BooleanFalseValues("no","n","false","f")]
    [JsonPropertyName("can_mail_order?")]
    public bool CanMailOrder  { get; set; }
}

public record struct ZipCode
{
    public int SingleValue { get; set; }
    public (int Min, int Max)? RangeValue { get; set; }
    
    public static implicit operator ZipCode(int value) 
        => new ZipCode { SingleValue = value };
    
    public static implicit operator ZipCode((int Min, int Max) range)  
        => new ZipCode { RangeValue = range };
}


public class ZipCodesConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        var result = new List<ZipCode>();
        var strZipCodeList = text?.Replace(" ","").Split(',');
        foreach (var strZipCode in strZipCodeList)
        {
            if (strZipCode.Contains('-'))
            {
                var parts = strZipCode.Split('-');
                result.Add((int.Parse(parts[0]), int.Parse(parts[1])));
            }
            else
            {
                result.Add(int.Parse(strZipCode));
            }
        }
        
        return result;
    }
}

public class CustomerSatisfactionScoreConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        if (decimal.TryParse(text, out decimal result))
        {
            return result;
        }

        return null; 
    }
}