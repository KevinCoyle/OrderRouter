using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace OrderRouter.Api.Models;

public enum ProductCategory
{
    AnkleBrace,
    BackBrace,
    BloodPressureMonitor,
    Cane,
    CervicalCollar,
    Commode,
    CompressionStockings,
    CPAP,
    CPMMachine,
    Crutches,
    GlucoseMeter,
    HeatingPad,
    HospitalBed,
    IceMachine,
    KneeScooter,
    Nebulizer,
    Oxygen,
    PatientLift,
    Rollator,
    ShowerChair,
    TensUnit,
    TractionDevice,
    Walker,
    Wheelchair,
}

public class ProductCategoryConverter : EnumConverter
{
    public ProductCategoryConverter() : base(typeof(ProductCategory))
    {
    }

    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        var result = text
                                        .Split(',')
                                        .Select(x => Enum.Parse<ProductCategory>(x.Replace(" ", ""), ignoreCase: true))
                                        .ToList();
        
        if (memberMapData.Type == typeof(List<ProductCategory>))
        {
            return result;
        }
        
        return result.FirstOrDefault();
    }
}