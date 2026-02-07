using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reserva.Api.Security;
using Reserva.Application.Extensions;
using Reserva.Domain.Extensions;
using Reserva.Entity;
using Reserva.Repository.Data;
using Reserva.Repository.Extensions;
using Reserva.Repository.Security;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

//conexion a base de datos desde secretos de usuario
var isProduction = false; //builder.Environment.IsProduction() || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CN_CONECTION"));

var connectionString = Environment.GetEnvironmentVariable("CN_CONECTION")?.Trim() ?? configuration["ConexionString"];

Console.WriteLine($"Connection String: {connectionString}");

// Configurar ReservaCanchasContext
if (isProduction)
    builder.Services.AddDbContext<ReservaCanchasContext>(options => options.UseSqlServer(connectionString));
else
    builder.Services.AddDbContext<ReservaCanchasContext>(options => options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4, 7))));

//Sentry
//builder.WebHost.UseSentry(o =>
//{
//    o.Dsn = Environment.GetEnvironmentVariable("SENTRY_DSN") ?? configuration.GetValue<string>("Sentry:Dsn");
//    o.Debug = true; // Opcional: útil en desarrollo
//    o.TracesSampleRate = 1.0; // Captura 100% del rendimiento, ajusta en producción
//});

// Cors
builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
{
    builder.WithOrigins("https://reserva-canchas-admin-web.vercel.app", "http://localhost:4200") //ANTES -> (configuration.GetValue<string>("AllowedHosts")!)
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

// Controllers
builder.Services.AddControllers();
// Endpoints
builder.Services.AddEndpointsApiExplorer();

// Repositories
builder.Services.UseRepositories(
    connectionString,
    configuration["AuditOptions:ApiUrl"]!,
    typeof(Program).Assembly.GetName().Name!,
    isProduction
 );

// Domain Services
builder.Services.UseDomainServices();

// Culqi Service para integración de pagos
builder.Services.AddHttpClient<Reserva.Domain.Services.Culqi.CulqiService>();
// Cloudflare R2 Storage Service para almacenamiento de imágenes
builder.Services.AddSingleton<Reserva.Domain.Services.Storage.IStorageService, Reserva.Domain.Services.Storage.CloudflareR2StorageService>();

// Security
builder.Services.AddHttpContextAccessor();
builder.Services.UseSecurity(configuration, isProduction);
// Application Services
builder.Services.UseApplicationServices();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//configuracion TkenProvider para resetear contrasenia
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
{
    opt.TokenLifespan = TimeSpan.FromHours(1); // valido por 1 horas
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Cors
app.UseCors("CorsPolicy");

// Authentication
app.UseAuthentication();

// Authorization
app.UseAuthorization();

//Sentry
//app.Use(async (context, next) =>
//{
//    var user = context.User;

//    if (user?.Identity?.IsAuthenticated == true)
//    {
//        SentrySdk.ConfigureScope(scope =>
//        {
//            scope.SetTag("Modulo", "Reclutamiento");
//            scope.SetTag("usuario", user.FindFirst("DisplayName")?.Value);
//            scope.SetTag("Connection", context.Connection.RemoteIpAddress?.ToString());
//            scope.SetTag("Empresa", "RunaIT");
//        });
//    }

//    await next();
//});

app.MapControllers();

app.Run();
