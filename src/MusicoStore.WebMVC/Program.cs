using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MusicoStore.BusinessLayer.Middleware;
using MusicoStore.BusinessLayer.Services;
using MusicoStore.DataAccessLayer;
using MusicoStore.DataAccessLayer.Identity;
using MusicoStore.DataAccessLayer.Repository;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Infrastructure;
using MusicoStore.Domain.Interfaces.Repository;
using MusicoStore.Domain.Interfaces.Service;
using MusicoStore.Mongo.Logging;
using WebMVC.Infrastructure;
using WebMVC.Mapping;
using WebMVC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var envName = builder.Environment.EnvironmentName;

Console.WriteLine($"Env Name: {envName}");

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options
        .UseInMemoryDatabase("MusicoStore")
        .LogTo(a => Console.WriteLine(a), LogLevel.Debug)
        .EnableSensitiveDataLogging(true);
});

builder.Services.AddMemoryCache();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRepository<ProductCategory>, ProductCategoryRepository>();
builder.Services.AddScoped<IRepository<ProductEditLog>, ProductEditLogRepository>();
builder.Services.AddScoped<IRepository<Manufacturer>, ManufacturerRepository>();
builder.Services.AddScoped<IRepository<Address>, AddressRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IRepository<Storage>, StorageRepository>();
builder.Services.AddScoped<IRepository<Customer>, CustomerRepository>();
builder.Services.AddScoped<IRepository<Order>, OrderRepository>();
builder.Services.AddScoped<IRepository<OrderedProduct>, OrderedProductRepository>();
builder.Services.AddScoped<IRepository<OrderState>, OrderStateRepository>();
builder.Services.AddScoped<IOrderStatusLogRepository, OrderStatusLogRepository>();
builder.Services.AddScoped<IRepository<GiftCard>, GiftCardRepository>();
builder.Services.AddScoped<IGiftCardCouponRepository, GiftCardCouponRepository>();
builder.Services.AddScoped<IProductCategoryAssignmentRepository, ProductCategoryAssignmentRepository>();

builder.Services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();

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
builder.Services.AddScoped<ICurrencyConversionService, CurrencyConversionService>();

builder.Services.AddScoped<IEmailSender, EmailSender>();

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


builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
    cfg.AddProfile<MusicoStore.Domain.Mapping.MappingProfile>();
});

builder.Services.AddIdentity<LocalIdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

builder.Services.ConfigureApplicationCookie(options => { options.LoginPath = "/Account/Login"; });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseMiddleware<RequestLoggingMiddleware>("[MVC]");

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await SeedData.EnsureSeededAsync(db);
    await SeedData.SeedTestLoginWithOrdersAsync(scope.ServiceProvider, db);
    await SeedData.SeedRoles(scope.ServiceProvider);
}

app.Run();
