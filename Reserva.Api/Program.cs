using Microsoft.EntityFrameworkCore;
using Reserva.Application.Extensions;
using Reserva.Domain.Extensions;
using Reserva.Entity.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

//conexion a base de datos desde secretos de usuario
builder.Services.AddDbContext<ReservaCanchasContext>(options => options.UseSqlServer(configuration["ConexionString"]));

// Controllers
builder.Services.AddControllers();
// Endpoints
builder.Services.AddEndpointsApiExplorer();

// Domain Services
builder.Services.UseDomainServices();
// Application Services
builder.Services.UseApplicationServices();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
