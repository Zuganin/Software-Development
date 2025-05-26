

using FileStoringService.Application.Interfaces;
using FileStoringService.Infrastructure;
using FileStoringService.Domain.Interfaces;
using FileStoringService.Infrastructure.Data;
using FileStoringService.Infrastructure.Repositories;
using FileStoringService.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Mvc.Controllers;
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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "File Storage API", Version = "v1" });

    // Ограничение на свои контроллеры
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (apiDesc.ActionDescriptor is not ControllerActionDescriptor descriptor)
            return false;

        return descriptor.ControllerTypeInfo.Namespace?.Contains("FileStoringService") == true;
    });

    c.AddServer(new OpenApiServer { Url = "/api/files" });
});



var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger(c =>
{
    c.RouteTemplate = "api/files/swagger/{documentName}/swagger.json";
});
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/api/files/swagger/v1/swagger.json", "File Storage API");
    c.RoutePrefix = "api/files/swagger";
});



app.UseHttpsRedirection();
app.MapControllers();
app.Run();

