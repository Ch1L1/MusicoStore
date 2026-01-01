using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicoStore.BusinessLayer.Services;
using MusicoStore.DataAccessLayer;
using MusicoStore.DataAccessLayer.Identity;
using MusicoStore.DataAccessLayer.Repository;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Infrastructure;
using MusicoStore.Domain.Interfaces.Repository;
using MusicoStore.Domain.Interfaces.Service;
using WebMVC.Infrastructure;
using WebMVC.Mapping;

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

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRepository<Customer>, CustomerRepository>();
builder.Services.AddScoped<IRepository<ProductEditLog>, ProductEditLogRepository>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IStoragePathProvider, WebRootPathProvider>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();


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
}

app.Run();
