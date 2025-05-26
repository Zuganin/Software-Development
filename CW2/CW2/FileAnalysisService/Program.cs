using FileAnalysisService.Application.Interfaces;
using FileAnalysisService.Application.Services;
using FileAnalysisService.Domain.Interfaces;
using FileAnalysisService.Infrastructure.Clients;
using FileAnalysisService.Infrastructure.Data;
using FileAnalysisService.Infrastructure.Repositories;
using FileAnalysisService.Infrastructure.WordCloud;
using FileStoringService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;


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
    c.SwaggerDoc("v1", new()
    {
        Title = "FileAnalysis API",
        Version = "v1",
        Description = "Сервис анализа текста и генерации облака слов"
    });
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TextAnalysisDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseStaticFiles();       // serve word cloud PNGs from wwwroot

app.MapControllers();

app.Run();