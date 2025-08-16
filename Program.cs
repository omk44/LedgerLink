// Path: LedgerLink/Program.cs
using System.Globalization; // Required for CultureInfo
using Microsoft.AspNetCore.Localization; // Required for RequestLocalizationOptions
using Microsoft.EntityFrameworkCore;
using LedgerLink.Data;
using LedgerLink.Models; // Required for ShopSettings
using Microsoft.Extensions.Options; // Required for IOptions
 // Your DbContext namespace
using Npgsql.EntityFrameworkCore.PostgreSQL;
using LedgerLink.Interface; // Your interfaces
using LedgerLink.Services; // Your service implementations
using System;

var builder = WebApplication.CreateBuilder(args);
// --- Configure Application Culture for India (en-IN) ---
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var defaultCulture = new CultureInfo("en-IN"); // English (India) culture
    var supportedCultures = new[]
    {
        defaultCulture,
        new CultureInfo("en-US"), // Example: also support US English
        // Add other cultures if needed
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

// --- Add Session Services ---
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Sets how long a session can be inactive before expiring
    options.Cookie.HttpOnly = true; // Makes the session cookie inaccessible to client-side scripts (security)
    options.Cookie.IsEssential = true; // Marks the cookie as essential for the app to function (GDPR compliance)
});

// --- Register your custom repositories and services here ---
builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();
builder.Services.AddScoped<IPaymentRepo, PaymentRepo>();
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<ITransactionRepo, TransactionRepo>();
builder.Services.AddTransient<QrCodeService>();

builder.Services.Configure<ShopSettings>(builder.Configuration.GetSection("ShopSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
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

// --- CRITICAL FIX: Add Session Middleware Here ---
// This middleware MUST be placed after app.UseRouting()
// and before app.MapControllerRoute() or app.UseEndpoints().
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();