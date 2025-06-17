using Microsoft.EntityFrameworkCore;
using PaymentsService.Infrastructure.DbContext;
using PaymentsService.Infrastructure.Repositories;
using PaymentsService.Application.Interfaces;
using PaymentsService.Application.Services;
using PaymentsService.Domain.Model.Interfaces;
using PaymentsService.Infrastructure.Background;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuration: считываем connection string из appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Регистрация DbContext для PostgreSQL
builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseNpgsql(connectionString));

// 3. Регистрация репозитория и сервиса
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
builder.Services.AddScoped<IInboxRepository, InboxRepository>();
builder.Services.AddScoped<IAccountService, AccountService>(provider =>
{
    var accountRepo = provider.GetRequiredService<IAccountRepository>();
    var outboxRepo = provider.GetRequiredService<IOutboxRepository>();
    var dbContext = provider.GetRequiredService<PaymentsDbContext>();
    var transactionRepo = provider.GetRequiredService<TransactionRepository>();
    return new AccountService(accountRepo, outboxRepo, dbContext, transactionRepo);
});
builder.Services.AddScoped<TransactionRepository>();
builder.Services.AddHostedService<OutboxKafkaPublisher>();
builder.Services.AddHostedService<InboxEventProcessor>();
builder.Services.AddHostedService<InboxKafkaConsumer>();

// 4. Добавление контроллеров и Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "PaymentsService API", Version = "v1" });
    options.AddServer(new Microsoft.OpenApi.Models.OpenApiServer { Url = "/payments" }); // только для Kong
});

// 5. Постройка приложения
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
    db.Database.EnsureDeleted(); // каждый раз удаляет БД
    db.Database.EnsureCreated(); // и создаёт заново по текущей модели
}

// 6. Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();

// 7. Маршрутизация
app.MapControllers();

// 8. Запуск
app.Run();