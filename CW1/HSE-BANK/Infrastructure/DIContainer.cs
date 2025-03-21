using HSE_BANK.Commands;
using HSE_BANK.Export;
using HSE_BANK.Facades;
using HSE_BANK.Factories;
using HSE_BANK.Interfaces.Command;
using HSE_BANK.Interfaces.Export;
using HSE_BANK.Interfaces.IFactories;
using HSE_BANK.Interfaces.Repository;
using HSE_BANK.Proxy.Cache;
using HSE_BANK.Proxy.InMemory;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace HSE_BANK.Infrastructure;

public static class DiContainer
{
    private static IServiceProvider? _services;

    public static IServiceProvider Services => _services ??= CreateServiceProvider();

    private static IServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.AddMemoryCache();

        services.AddSingleton<ICommands, CreateBankAccountCommand>();
        services.AddSingleton<ICommands, CreateCategoryCommand>();
        services.AddSingleton<ICommands, CreateOperationCommand>();

        services.AddSingleton<TimedCommandDecorator>();

        services.AddSingleton<IExportVisitor, CsvExportVisitor>();
        services.AddSingleton<IExportVisitor, JsonExportVisitor>();
        services.AddSingleton<IExportVisitor, YamlExportVisitor>();
        
        services.AddSingleton<Exporter>();
        
        services.AddSingleton<IBankAccountFactory , BankAccountFactory>();
        services.AddSingleton<ICategoryFactory, CategoryFactory>();
        services.AddSingleton<IOperationFactory, OperationFactory>();

        services.AddSingleton<InMemoryBankAccountRepository>();
        services.AddSingleton<InMemoryCategoryRepository>();
        services.AddSingleton<InMemoryOperationRepository>();

        services.AddSingleton<IBankAccountRepository>(provider =>
            new CacheBankAccountRepository(
                provider.GetRequiredService<InMemoryBankAccountRepository>(),
                provider.GetRequiredService<IMemoryCache>()));
        services.AddSingleton<ICategoryRepository>(provider =>
            new CacheCategoryRepository(
                provider.GetRequiredService<InMemoryCategoryRepository>(),
                provider.GetRequiredService<IMemoryCache>()));
        services.AddSingleton<IOperationRepository>(provider =>
            new CacheOperationRepository(
                provider.GetRequiredService<InMemoryOperationRepository>(),
                provider.GetRequiredService<IMemoryCache>()));
        
        services.AddSingleton<IBankAccountFacade, BankAccountFacade>();
        services.AddSingleton<ICategoryFacade, CategoryFacade>();
        services.AddSingleton<IOperationFacade, OperationFacade>();
        services.AddSingleton<IAnalysis, AnalysisFacade>(); ;

        services.AddTransient<CreateBankAccountCommand>();
        services.AddTransient<CreateCategoryCommand>();
        services.AddTransient<CreateOperationCommand>();
        
        services.AddSingleton<Menu>();
        
        return services.BuildServiceProvider();
    }
}