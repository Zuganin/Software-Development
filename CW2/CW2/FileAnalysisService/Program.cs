using FileAnalisysService.Application.Services;
using FileAnalisysService.Domain.Interfaces;
using FileAnalisysService.Infrastructure.Clients;
using FileAnalisysService.Infrastructure.Data;
using FileAnalisysService.Infrastructure.Repositories;
using FileAnalisysService.Infrastructure.WordCloud;
using FileStoringService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// 1. Configuration & DbContext
builder.Services.AddDbContext<TextAnalysisDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TextAnalysisDb")));

// 2. Infrastructure
builder.Services.AddScoped<IFileAnalysisRepository, FileAnalysisRepository>();
builder.Services.AddScoped<IWordCloudGenerator, QuickChartWordCloudGenerator>();

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
builder.Services.AddSwaggerGen();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TextAnalysisDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();       // serve word cloud PNGs from wwwroot

app.MapControllers();

app.Run();