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
builder.Services.AddScoped<IAccountService, AccountService>(provider =>
{
    var accountRepo = provider.GetRequiredService<IAccountRepository>();
    var outboxRepo = provider.GetRequiredService<IOutboxRepository>();
    var dbContext = provider.GetRequiredService<PaymentsDbContext>();
    return new AccountService(accountRepo, outboxRepo, dbContext);
});

// 4. Добавление контроллеров и Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<OutboxKafkaPublisher>();

// 5. Постройка приложения
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
    db.Database.EnsureCreated();
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