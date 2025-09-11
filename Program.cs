// Path: LedgerLink/Program.cs
using Microsoft.EntityFrameworkCore;
using LedgerLink.Data; // Your DbContext namespace
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.AspNetCore.Builder;
using LedgerLink.ViewModels;
using LedgerLink.Interface; // Your interfaces
using LedgerLink.Services; // Your service implementations
using System;
using System.Globalization; // Required for CultureInfo
using Microsoft.AspNetCore.Localization; // Required for RequestLocalizationOptions
using Microsoft.Extensions.Options; // Required for IOptions (for ShopSettings)
// REMOVED: using Microsoft.Extensions.Configuration; // No longer explicitly needed here

var builder = WebApplication.CreateBuilder(args);

// --- CRITICAL FIX: REMOVE these explicit configuration loading lines ---
// WebApplication.CreateBuilder(args) already loads appsettings.json by default.
// builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//                      .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);


// Configure Application Culture for India (en-IN)
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var defaultCulture = new CultureInfo("en-IN");
    var supportedCultures = new[]
    {
        defaultCulture,
        new CultureInfo("en-US"),
    };

    options.DefaultRequestCulture = new RequestCulture(defaultCulture);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});


// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Session Services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register ShopSettings from configuration
builder.Services.Configure<ShopSettings>(builder.Configuration.GetSection("ShopSettings"));

// Register your custom repositories and services
builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();
builder.Services.AddScoped<IPaymentRepo, PaymentRepo>(); // Corrected from IPaymentRepo
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<ITransactionRepo, TransactionRepo>(); // Corrected from ITransactionRepo
builder.Services.AddTransient<QrCodeService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IFestivalRepo, FestivalRepo>();
builder.Services.AddScoped<IDiscountRuleRepo, DiscountRuleRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRequestLocalization(); // Must be before UseRouting()

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable Session Middleware
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();