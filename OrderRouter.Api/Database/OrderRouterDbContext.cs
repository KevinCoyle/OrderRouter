using System.Globalization;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using OrderRouter.Api.Models;

namespace OrderRouter.Api.Database;

public class OrderRouterDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }

    public OrderRouterDbContext(DbContextOptions<OrderRouterDbContext> options, IConfiguration configuration) 
        : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderRouterDbContext).Assembly);
        modelBuilder.Entity<Supplier>()
            .Property(s => s.ProductCategories)
            .HasConversion(
                pc => JsonSerializer.Serialize(pc, (JsonSerializerOptions)null),
                pc => JsonSerializer.Deserialize<List<ProductCategory>>(pc, (JsonSerializerOptions)null)
            )
            .HasColumnType("json");

        modelBuilder.Entity<Supplier>()
            .Property(s => s.ZipCodes)
            .HasConversion(
                zc => JsonSerializer.Serialize(zc, new JsonSerializerOptions()),
                zc => JsonSerializer.Deserialize<List<ZipCode>>(zc, new JsonSerializerOptions())
            )
            .HasColumnType("json");
        
        base.OnModelCreating(modelBuilder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSeeding((context, _) =>
            {
                var productsFilepath = _configuration["FileSettings:ProductsListRelativeFilepath"];
                var suppliersFilepath = _configuration["FileSettings:SuppliersListRelativeFilepath"];
                var products = context.Set<Product>();
                var suppliers = context.Set<Supplier>();

                var csvConf = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    TrimOptions = TrimOptions.Trim,
                    MissingFieldFound = null
                };

                if (!products.Any() && productsFilepath != null)
                {
                    using (var reader = new StreamReader(productsFilepath))
                    using (var csv = new CsvReader(reader, csvConf))
                    {
                        csv.Context.TypeConverterOptionsCache.GetOptions<ProductCategory>().EnumIgnoreCase = true;
                        var records = csv.GetRecords<Product>().ToList();

                        products.AddRange(records.DistinctBy(x => x.Id).ToList());
                        context.SaveChanges();
                    }
                }

                if (!suppliers.Any() && suppliersFilepath != null)
                {
                    using (var reader = new StreamReader(suppliersFilepath))
                    using (var csv = new CsvReader(reader, csvConf))
                    {
                        csv.Context.TypeConverterOptionsCache.GetOptions<ProductCategory>().EnumIgnoreCase = true;
                        var records = csv.GetRecords<Supplier>().ToList();

                        suppliers.AddRange(records.DistinctBy(x => x.Id).ToList());
                        context.SaveChanges();
                    }
                }
            })
            .UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                var productsFilepath = _configuration["FileSettings:ProductsListRelativeFilepath"];
                var suppliersFilepath = _configuration["FileSettings:SuppliersListRelativeFilepath"];
                var products = context.Set<Product>();
                var suppliers = context.Set<Supplier>();

                var csvConf = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    TrimOptions = TrimOptions.Trim,
                    MissingFieldFound = null
                };

                if (!products.Any() && productsFilepath != null)
                {
                    using (var reader = new StreamReader(productsFilepath))
                    using (var csv = new CsvReader(reader, csvConf))
                    {
                        csv.Context.TypeConverterOptionsCache.GetOptions<ProductCategory>().EnumIgnoreCase = true;
                        var records = csv.GetRecords<Product>().ToList();

                        products.AddRange(records.DistinctBy(x => x.Id).ToList());
                        await context.SaveChangesAsync(cancellationToken);
                    }
                }
                if (!suppliers.Any() && suppliersFilepath != null)
                {
                    using (var reader = new StreamReader(suppliersFilepath))
                    using (var csv = new CsvReader(reader, csvConf))
                    {
                        csv.Context.TypeConverterOptionsCache.GetOptions<ProductCategory>().EnumIgnoreCase = true;
                        var records = csv.GetRecords<Supplier>().ToList();

                        suppliers.AddRange(records.DistinctBy(x => x.Id).ToList());
                        await context.SaveChangesAsync(cancellationToken);
                    }
                }
            });
    }
    
}