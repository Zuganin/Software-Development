using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using mini_hw_2.Application.Interfaces;
using mini_hw_2.Application.Services;
using mini_hw_2.Infratructure.Persistence;
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IAnimalRepository, InMemoryAnimalRepository>();
builder.Services.AddSingleton<IEnclosureRepository, InMemoryEnclosureRepository>();
builder.Services.AddSingleton<IFeedingScheduleRepository, InMemoryFeedingScheduleRepository>();
builder.Services.AddSingleton<IAnimalTransferService, AnimalTransferService>();
builder.Services.AddSingleton<IStatisticsService, ZooStatisticsService>();

var app = builder.Build();

// Enable middleware
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
        app.UseSwagger();
        app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Urls.Add("http://localhost:8080");

app.Run();
