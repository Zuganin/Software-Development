

using FileStoringService.Application.Interfaces;
using FileStoringService.Infrastructure;
using FileStoringService.Domain.Interfaces;
using FileStoringService.Infrastructure.Data;
using FileStoringService.Infrastructure.Repositories;
using FileStoringService.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Infrastructure
builder.Services.AddScoped<IFileRepository, EfFileRepository>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
// Сервис хранения файлов
builder.Services.AddScoped<IFileStoringService, FileStorageService>();

// 3. Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FileStoringService API",
        Version = "v1",
        Description = "Сервис для хранения файлов"
    });
});

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.MapControllers();
app.Run();

