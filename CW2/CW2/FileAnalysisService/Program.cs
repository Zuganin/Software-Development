using System.Reflection;
using FileAnalysisService.Application.Interfaces;
using FileAnalysisService.Application.Services;
using FileAnalysisService.Domain.Interfaces;
using FileAnalysisService.Infrastructure.Clients;
using FileAnalysisService.Infrastructure.Data;
using FileAnalysisService.Infrastructure.Repositories;
using FileAnalysisService.Infrastructure.WordCloud;
using FileStoringService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Controllers;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuration & DbContext
builder.Services.AddDbContext<TextAnalysisDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Infrastructure
builder.Services.AddScoped<IFileAnalysisRepository, FileAnalysisRepository>();
builder.Services.AddScoped<IWordCloudGenerator, QuickChartWordCloudGenerator>();
builder.Services.AddScoped<IAnalyzeFileService, AnalyzeFileService>();

// 3. External FileStoringService client
builder.Services.AddHttpClient<IFileStoringService, FileStorageHttpClient>(client =>
{
    var baseUrl = builder.Configuration.GetValue<string>("FileStoringService:BaseUrl");
    client.BaseAddress = new Uri(baseUrl);
});

// 4. Application Service
builder.Services.AddScoped<AnalyzeFileService>();

// 5. Controllers, Swagger & Static Files
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "File Analysis API",
        Version = "v1"
    });


    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);


    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (apiDesc.ActionDescriptor is not ControllerActionDescriptor descriptor)
            return false;

        // Оставляем только контроллеры анализа
        return descriptor.ControllerTypeInfo.Namespace?.Contains("FileAnalysisService") == true;
    });

    // 🔧 Базовый путь для всех маршрутов в Swagger
    c.AddServer(new OpenApiServer { Url = "/api/analyze" });
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TextAnalysisDbContext>();
    db.Database.EnsureCreated();
}


app.UseSwagger(c =>
{
    c.RouteTemplate = "api/analyze/swagger/{documentName}/swagger.json";
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/api/analyze/swagger/v1/swagger.json", "File Analysis API");
    c.RoutePrefix = "api/analyze/swagger";
});

// In Program.cs or wherever your host is built
builder.Logging.AddConsole();
app.UseHttpsRedirection();
app.UseStaticFiles(); // PNG облака слов
app.MapControllers();
app.Run();
