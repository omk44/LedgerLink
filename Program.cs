using System;
using System.Globalization;
using LedgerLink.Data;
using LedgerLink.Interface;
using LedgerLink.Services;
using LedgerLink.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add Health Checks for Render deployment
builder.Services.AddHealthChecks();

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

// Add Session Services with enhanced security
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // Only require HTTPS in production when not in Docker
    options.Cookie.SecurePolicy = builder.Environment.IsProduction() && Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true"
        ? Microsoft.AspNetCore.Http.CookieSecurePolicy.Always
        : Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
    options.Cookie.Name = "LedgerLink.Session";
});

// Register your custom repositories and services
builder.Services.AddScoped<IShopRepo, ShopRepo>();
builder.Services.AddScoped<IAdminRepo, AdminRepo>();
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

// Only redirect to HTTPS in production when not in Docker
if (app.Environment.IsProduction() && Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

// Enable Session Middleware
app.UseSession();

// Add health check endpoint for Render
app.MapHealthChecks("/health");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();