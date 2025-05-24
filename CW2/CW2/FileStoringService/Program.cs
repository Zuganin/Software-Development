using System;
using System.Threading.Tasks;
using FileStoringService.Application.DTOs;
using FileStoringService.Application.Interfaces;
using FileStoringService.Infrastructure.Data;
using FileStoringService.Infrastructure.Repositories;
using FileStoringService.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .ConfigureServices((ctx, services) =>
            {
                // 1. DbContext
                services.AddDbContext<AppDbContext>(opts =>
                    opts.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=db"));

                // 2. Infrastructure
                services.AddScoped<IFileRepository, EfFileRepository>();
                services.AddScoped<IUnitOfWork, EfUnitOfWork>();

                // 3. Application
                services.AddScoped<IFileStoringService, FileStorageService>();
            })
            .Build();

        // Открываем scope для инициализации БД и работы с сервисами
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        // 4. Убедиться, что база и таблицы существуют
        var db = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        try
        {
            await db.Database.EnsureCreatedAsync();
            logger.LogInformation("Database created or already exists.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating database");
            return;
        }

        // 5. Получаем сервис для работы с файлами
        var fileService = services.GetRequiredService<IFileStoringService>();

        // 6. Определяем пути к файлам: из args или жёстко
        string path1, path2;
        if (args.Length >= 2)
        {
            path1 = args[0];
            path2 = args[1];
        }
        else
        {
            Console.Write("Enter full path to file: ");
            path1 = "/Users/vadimzenin/RiderProjects/КПО/Software-Development/CW2/CW2/Test.txt";
            Console.Write("Enter full path to file: ");
            path2 = "/Users/vadimzenin/RiderProjects/КПО/Software-Development/CW2/CW2/Test1.txt";
            
        }
        try
        {
            // 7. Сохраняем (или получаем существующий) первый файл
            FileDto meta1 = await fileService.SaveFileAsync(path1);
            Console.WriteLine($"First file ID: {meta1.Id}, Location: {meta1.Location}");

            // 8. Сохраняем второй файл
            FileDto meta2 = await fileService.SaveFileAsync(path2);
            Console.WriteLine($"Second file ID: {meta2.Id}, Location: {meta2.Location}");

            // 9. Покажем список всех метаданных
            Console.WriteLine("\nAll files metadata:");
            var all = await fileService.GetAllFilesMetadataAsync();
            foreach (var m in all)
            {
                Console.WriteLine($"- {m.Id} | {m.Name} | {m.Hash} | {m.Location} | {m.UploadDate}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing files");
        }
    }
}


