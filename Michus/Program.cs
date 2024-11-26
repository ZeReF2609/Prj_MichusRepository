using Michus.Models;
using Michus.Service;
using Michus.DAO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext
builder.Services.AddDbContext<MichusContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("cn1")));

// Agregar LoginService al contenedor de servicios
builder.Services.AddScoped<LoginService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("cn1");
    var logger = provider.GetRequiredService<ILogger<LoginService>>();
    return new LoginService(connectionString, logger);
});

// Agregar MenuService al contenedor de servicios
builder.Services.AddScoped<MenuService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("cn1");
    return new MenuService(connectionString);
});

// Agregar ProductoDAO al contenedor de servicios
builder.Services.AddScoped<ProductoDAO>();

builder.Services.AddScoped<ClientesDAO>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("cn1");
    return new ClientesDAO(connectionString);
});

builder.Services.AddScoped<EmpleadoDAO>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("cn1");
    return new EmpleadoDAO(connectionString);
});

//servicio para el envio del correo
builder.Services.AddTransient<CorreoHelper>();

// Agregar controladores y vistas
builder.Services.AddControllersWithViews();

// Configuración de autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Login";
        options.LogoutPath = "/Login/Salir";
        options.AccessDeniedPath = "/Login/Login";
        options.Cookie.Name = "GP_Session";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
    });

var app = builder.Build();

// Configuración del entorno
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Configuración de rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Ecommerce}/{action=EcommerceProductosIndex}/{id?}");

app.Run();
