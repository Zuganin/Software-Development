using Microsoft.EntityFrameworkCore;

using OrdersService.Infrastructure.DatabaseContext;
using OrdersService.Infrastructure.Repositories;
using OrdersService.Application.Services;
using OrdersService.Infrastructure.Background;

var builder = WebApplication.CreateBuilder(args);


// Подключение к Postgres
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseNpgsql(connectionString));

// DI
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<OutboxRepository>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddHostedService<OutboxKafkaPublisher>();
builder.Services.AddHostedService<PaymentStatusKafkaConsumer>();

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "OrdersService API", Version = "v1" });
    options.AddServer(new Microsoft.OpenApi.Models.OpenApiServer { Url = "/orders" }); // только для Kong
});

var app = builder.Build();

// Автоматическое создание БД
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    db.Database.EnsureDeleted(); 
    db.Database.EnsureCreated(); 
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
logger.LogInformation("OrdersService ASP.NET app is starting and ready to accept requests.");

app.Run();