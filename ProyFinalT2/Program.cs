using ProyFinalT2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using ProyFinalT2.Servicios;
using ProyFinalT2.Models;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// politicaUsuariosAutenticados es una política que requiere que el usuario esté autenticado
var politicaUsuariosAutenticados = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser() // requiere que el usuario esté autenticado
    .Build(); // crea la política

// Agrego filtros para que todas las vistas requieran autenticación
builder.Services.AddControllersWithViews(opciones =>
{
    opciones.Filters.Add(new AuthorizeFilter(politicaUsuariosAutenticados));
}).AddJsonOptions(opciones =>
{
    opciones.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Configuro la base de datos para usar SQL Server 
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer("name=DefaultConnection"));

//// Configuración de la autenticación con cuentas de Microsoft
//builder.Services.AddAuthentication().AddMicrosoftAccount(opciones =>
//{
//    opciones.ClientId = builder.Configuration["MicrosoftClientId"];
//    opciones.ClientSecret = builder.Configuration["MicrosoftSecretId"];
//});

// IdentityUser y IdentityRole representan a los usuarios y roles de Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(opciones =>
{
    // No es necesario confirmar la cuenta para iniciar sesión
    opciones.SignIn.RequireConfirmedAccount = false;
}).AddEntityFrameworkStores<ApplicationDbContext>() // Almacena datos con Entity Framework
  .AddDefaultTokenProviders(); // Genera tokens para confirmar cuentas y otras acciones

// Configuro vistas personalizadas para iniciar sesión y acceso denegado
builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
    opciones =>
    {
        opciones.LoginPath = "/usuarios/login"; // Ruta personalizada para iniciar sesión
        opciones.AccessDeniedPath = "/usuarios/login"; // Ruta para error de acceso denegado
    });

// Agrego servicios personalizados
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();
builder.Services.AddAutoMapper(typeof(Program)); // Configuración de AutoMapper

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Manejo de excepciones para producción
    app.UseHsts(); // HTTP Strict Transport Security (HSTS)
}

app.UseHttpsRedirection(); // Redirección de HTTP a HTTPS
app.UseStaticFiles(); // Sirve archivos estáticos

app.UseRouting(); // Habilita el enrutamiento de las solicitudes

// Habilita la autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Define la ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
