using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using HSE_BANK.Commands;
using HSE_BANK.Export;
using HSE_BANK.Facades;
using HSE_BANK.Infrastructure;
using HSE_BANK.Interfaces.Repository;
using HSE_BANK.Proxy.Cache;
using HSE_BANK.Proxy.InMemory;

public static class DIContainer
{
    private static IServiceProvider? _services;

    public static IServiceProvider Services => _services ??= CreateServiceProvider();

    private static IServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddSingleton<Menu>();
        
        services.AddMemoryCache();

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

        services.AddSingleton<IAnalysis, AnalysisFacade>();
        

        
        services.AddTransient<JsonExportVisitor>();
        services.AddTransient<YamlExportVisitor>();

        services.AddTransient<CreateBankAccountCommand>();
        services.AddTransient<CreateCategoryCommand>();
        services.AddTransient<CreateOperationCommand>();
        
        return services.BuildServiceProvider();
    }
    public static Menu Menu => Services.GetRequiredService<Menu>();
    
    public static IBankAccountFacade BankAccountFacade => Services.GetRequiredService<IBankAccountFacade>();
    
    public static ICategoryFacade CategoryFacade => Services.GetRequiredService<ICategoryFacade>();
    
    public static IOperationFacade OperationFacade => Services.GetRequiredService<IOperationFacade>();
    
    public static IAnalysis AnalyticsService => Services.GetRequiredService<IAnalysis>();
    
    public static CsvExportVisitor CsvExporter => Services.GetRequiredService<CsvExportVisitor>();
    public static JsonExportVisitor JsonExporter => Services.GetRequiredService<JsonExportVisitor>();
    
    public static YamlExportVisitor YamlExporter => Services.GetRequiredService<YamlExportVisitor>();
    
    public static CreateBankAccountCommand CreateBankAccountCommand => Services.GetRequiredService<CreateBankAccountCommand>();
    
    public static CreateCategoryCommand CreateCategoryCommand => Services.GetRequiredService<CreateCategoryCommand>();
    
    public static CreateOperationCommand CreateOperationCommand => Services.GetRequiredService<CreateOperationCommand>();
}