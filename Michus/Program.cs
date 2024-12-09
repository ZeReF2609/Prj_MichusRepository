using Michus.Models;
using Michus.Service;
using Michus.DAO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext
builder.Services.AddDbContext<MichusContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("cn1")));

// Configuración de Autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("R01")); // Solo usuarios con rol R01 pueden acceder
});

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

// Agregar ProductoDAO al contenedor de servicios
builder.Services.AddScoped<ProductoDao>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("cn1");
    return new ProductoDao(connectionString);
});

builder.Services.AddScoped<CuentaDAO>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("cn1");
    return new CuentaDAO(connectionString);
});

builder.Services.AddScoped<ClientesDAO>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("cn1");
    return new ClientesDAO(connectionString);
});

builder.Services.AddScoped<TipoDocumentoDAO>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("cn1");
    return new TipoDocumentoDAO(connectionString);
});

builder.Services.AddScoped<RolesDAO>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("cn1");
    return new RolesDAO(connectionString);
});

builder.Services.AddScoped<CuentaDAO>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("cn1");
    return new CuentaDAO(connectionString);
});

builder.Services.AddScoped<LoginCliService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("cn1");
    var logger = provider.GetRequiredService<ILogger<LoginCliService>>();
    return new LoginCliService(connectionString, logger);
});

builder.Services.AddScoped<EmpleadoDAO>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("cn1");
    return new EmpleadoDAO(connectionString);
});

builder.Services.AddScoped<ReservasDAO>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("cn1");
    return new ReservasDAO(connectionString);
});

// Servicio para el envío del correo
builder.Services.AddTransient<CorreoHelper>();

// Agregar controladores y vistas
builder.Services.AddControllersWithViews();

// Configuración de autenticación y cookies
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/LoginCli/LoginCli"; // Ruta de inicio de sesión
    options.LogoutPath = "/LoginCli/Salir"; // Ruta de cierre de sesión
    options.AccessDeniedPath = "/LoginCli/LoginCli"; // Ruta de acceso denegado
    options.Cookie.Name = "Michus_Session";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Configuración de sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configuración del entorno
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Usar autenticación, autorización y sesión
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Configuración de rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Ecommerce}/{action=ListarProductos}/{id?}");

// Agregar middleware personalizado para manejar acceso denegado
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 403) // Acceso Denegado
    {
        context.Response.Redirect("/LoginCli/LoginCli");
    }
});

app.Run();