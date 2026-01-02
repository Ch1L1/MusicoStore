using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using MusicoStore.BusinessLayer.Middleware;
using MusicoStore.BusinessLayer.Services;
using MusicoStore.DataAccessLayer;
using MusicoStore.DataAccessLayer.Repository;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Infrastructure;
using MusicoStore.Domain.Interfaces.Repository;
using MusicoStore.Domain.Interfaces.Service;
using MusicoStore.Domain.Mapping;
using MusicoStore.Mongo.Logging;
using MusicoStore.WebApi.Infrastructure;
using MusicoStore.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccessLayer(builder.Configuration);

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));

builder.Services.AddSingleton(resolver =>
{
    var settings = resolver
        .GetRequiredService<
            Microsoft.Extensions.Options.IOptions<MongoDbSettings>>()
        .Value;

    return settings;
});

builder.Services.AddSingleton<ILoggingRepository>(sp =>
{
    var settings = sp.GetRequiredService<MongoDbSettings>();
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("LoggingRepositoryInit");

    if (MongoLoggingProbe.CanConnect(settings, out var initError))
    {
        return new MongoLoggingRepository(settings);
    }

    logger.LogWarning(initError, "Mongo logging disabled: unable to connect/authenticate. Check MongoDb settings.");
    return new NoOpLoggingRepository();
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .AddXmlSerializerFormatters();

builder.Services.AddMemoryCache();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRepository<ProductCategory>, ProductCategoryRepository>();
builder.Services.AddScoped<IRepository<ProductEditLog>, ProductEditLogRepository>();
builder.Services.AddScoped<IRepository<Manufacturer>, ManufacturerRepository>();
builder.Services.AddScoped<IRepository<Address>, AddressRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IRepository<Storage>, StorageRepository>();
builder.Services.AddScoped<IRepository<Customer>, CustomerRepository>();
builder.Services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();
builder.Services.AddScoped<IRepository<Order>, OrderRepository>();
builder.Services.AddScoped<IRepository<OrderedProduct>, OrderedProductRepository>();
builder.Services.AddScoped<IRepository<OrderState>, OrderStateRepository>();
builder.Services.AddScoped<IOrderStatusLogRepository, OrderStatusLogRepository>(); 
builder.Services.AddScoped<IRepository<GiftCard>, GiftCardRepository>();
builder.Services.AddScoped<IGiftCardCouponRepository, GiftCardCouponRepository>();
builder.Services.AddScoped<IProductCategoryAssignmentRepository, ProductCategoryAssignmentRepository>();

builder.Services.AddScoped<ICustomerAddressService, CustomerAddressService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IManufacturerService, ManufacturerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IStoragePathProvider, WebRootPathProvider>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IGiftCardService, GiftCardService>();
builder.Services.AddScoped<IProductCategoryAssignmentService, ProductCategoryAssignmentService>();

builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MappingProfile>(); });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MusicoStore API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter the access token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "Token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ResponseFormatMiddleware>();
app.UseMiddleware<TokenAuthMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>("[API]");

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await SeedData.EnsureSeededAsync(db);
}

app.Run();
