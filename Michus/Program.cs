using Michus.Models;
using Michus.Service;
using Michus.DAO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configurar los servicios y el DbContext
builder.Services.AddScoped<LoginService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("cn1");
    var logger = provider.GetRequiredService<ILogger<LoginService>>();
    return new LoginService(connectionString, logger);
});

builder.Services.AddScoped<MenuService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("cn1");
    return new MenuService(connectionString);
});

builder.Services.AddScoped<ProductoDAO>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new ProductoDAO(configuration);
});

builder.Services.AddScoped<LoginCliService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("cn1");
    var logger = provider.GetRequiredService<ILogger<LoginCliService>>();
    return new LoginCliService(connectionString, logger);
});

builder.Services.AddDbContext<MichusContext>(options =>
{
    var configuration = builder.Configuration;
    var connectionString = configuration.GetConnectionString("cn1");
    options.UseSqlServer(connectionString);
});

// Agregar controladores y vistas
builder.Services.AddControllersWithViews();

// Configuraci�n de autenticaci�n y cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/LoginCli/LoginCli"; // Ruta de inicio de sesi�n
        options.LogoutPath = "/LoginCli/Salir"; // Ruta de cierre de sesi�n
        options.AccessDeniedPath = "/LoginCli/LoginCli"; // Ruta de acceso denegado
        options.Cookie.Name = "Michus_Session";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
    });

// Configuraci�n de sesi�n
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configuraci�n del entorno
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

// Usar autenticaci�n, autorizaci�n y sesi�n
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Configuraci�n de rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Ecommerce}/{action=ListarProductos}/{id?}");

app.Run();
