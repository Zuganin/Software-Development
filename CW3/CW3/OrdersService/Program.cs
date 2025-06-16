using Microsoft.EntityFrameworkCore;

using OrdersService.Infrastructure.DatabaseContext;
using OrdersService.Infrastructure.Repositories;
using OrdersService.Application.Services;
using OrdersService.Infrastructure.Background;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:8081");


// Подключение к Postgres
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseNpgsql(connectionString));

// DI
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<OutboxRepository>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddHostedService<OutboxKafkaPublisher>();

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.Urls.Add("http://0.0.0.0:80");
// Автоматическое создание БД
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();