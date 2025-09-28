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

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

//conexion a base de datos desde secretos de usuario
var connectionString = Environment.GetEnvironmentVariable("CN_CONECTION")?.Trim() ?? configuration["ConexionString"];

builder.Services.AddDbContext<ReservaCanchasContext>(options =>
    options.UseSqlServer(connectionString));

// Cors
builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
{
    builder.WithOrigins(configuration.GetValue<string>("AllowedHosts")!)
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
    typeof(Program).Assembly.GetName().Name!
 );

// Domain Services
builder.Services.UseDomainServices();
// Security
builder.Services.AddHttpContextAccessor();
builder.Services.UseSecurity(configuration);
// Application Services
builder.Services.UseApplicationServices();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//configuracion TkenProvider para resetear contraseña
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
{
    opt.TokenLifespan = TimeSpan.FromHours(1); // válido por 1 horas
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

app.UseAuthorization();

app.MapControllers();

app.Run();
