using Mekaniko_Secured.Data;
using Mekaniko_Secured.Repository;
using Mekaniko_Secured.Repository.IRepository;
using Mekaniko_Secured.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

// Configure session state storage in memory (useful for storing small amounts of user data between requests)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Session timeout after 30 minutes of inactivity
    options.Cookie.HttpOnly = true;                  // Make session cookies accessible only to the server
    options.Cookie.IsEssential = true;               // Ensure session cookie is essential and always included
});

// Configure cookie-based authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";              // Redirect to this path if the user is not authenticated
        options.AccessDeniedPath = "/Login/AccessDenied"; // Redirect to this path if the user is not authorized
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Set cookie expiration to 30 minutes
        options.SlidingExpiration = true;                 // Reset expiration time if the user is active
    });

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("Admin")); 
});

// Configure Entity Framework Core to use SQL Server
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection") 
    ));

// Register application services for dependency injection
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IInvoicePdfService, InvoicePdfService>();

// Repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
// Add email service
builder.Services.AddScoped<IEmailService, EmailService>();


// Razor Runtime Compilation
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

// Configure logging to use settings from appsettings.json and to log to the console
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();

var app = builder.Build(); 

// Configure the HTTP request pipeline

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); 
    app.UseHsts();                          
}

app.UseHttpsRedirection(); 
app.UseStaticFiles();       

app.UseRouting();

app.UseAuthentication(); // Enable authentication middleware to check user credentials
app.UseAuthorization();  // Enable authorization middleware to enforce policies and roles

app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();