using DocumentGenerationApplication.Claims;
using DocumentGenerationApplication.Data;
using DocumentGenerationApplication.Models.UserModel;
using DocumentGenerationApplication.Repository;
using DocumentGenerationApplication.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Xceed.Document.NET;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
//Log.Logger = new LoggerConfiguration()
//    .Enrich.FromLogContext()
//    .WriteTo.Console() // Log to console (useful for debugging)
//    .WriteTo.File("logs/myapp-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10)
//    .CreateLogger();

//builder.Host.UseSerilog();



// Add Authentication + Authorization
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";   // 👈 Redirect here if not logged in
        //options.AccessDeniedPath = "/Account/AccessDenied";
    });

// Register DbContext with MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36)) // Use your installed MySQL version
    ));


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ISalaryRepository, SalaryRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();

builder.Services.Configure<IdentityOptions>(options =>
{

    options.Password.RequiredLength = 5;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
});

//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
//app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Account}/{action=Login}");

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}");


app.MapGet("/test", (ILogger<Program> logger) =>
{
    logger.LogInformation("Test endpoint accessed at {Time}", DateTime.UtcNow);

    try
    {
        throw new Exception("Simulated error");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Something went wrong in /test");
    }

    return "Check logs!";
});



app.Run();
