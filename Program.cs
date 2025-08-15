// Path: LedgerLink/Program.cs

using Microsoft.EntityFrameworkCore;
using LedgerLink.Data; // Your DbContext namespace
using Npgsql.EntityFrameworkCore.PostgreSQL;
// REMOVE: using Microsoft.AspNetCore.Identity; // No longer needed
// REMOVE: using LedgerLink.Models; // ApplicationUser is no longer needed in Program.cs
// REMOVE: using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // No longer needed

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options => // Changed from AppDbContext<ApplicationUser>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// REMOVE: Identity services
// builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
//     .AddEntityFrameworkStores<AppDbContext>();

// --- Register your custom repositories and services here ---
// using LedgerLink.Interface;
// using LedgerLink.Services;
builder.Services.AddScoped<LedgerLink.Interface.ICustomerRepo, LedgerLink.Services.CustomerRepo>();
builder.Services.AddScoped<LedgerLink.Interface.IPaymentRepo, LedgerLink.Services.PaymentRepo>();
builder.Services.AddScoped<LedgerLink.Interface.IProductRepo, LedgerLink.Services.ProductRepo>();
builder.Services.AddScoped<LedgerLink.Interface.ITransactionRepo, LedgerLink.Services.TransactionRepo>();
builder.Services.AddTransient<LedgerLink.Services.QrCodeService>();


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

// REMOVE: Authentication and Authorization middleware
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();