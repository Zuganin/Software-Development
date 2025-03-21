using HSE_BANK.Commands;
using HSE_BANK.Export;
using HSE_BANK.Facades;
using HSE_BANK.Infrastructure;
using HSE_BANK.Interfaces.Export;
using HSE_BANK.Interfaces.IFactories;
using HSE_BANK.Interfaces.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Infrastructure;

public class DiContainerTests
{
    private readonly IServiceProvider _services;

    public DiContainerTests()
    {
        _services = DiContainer.Services;
    }

    [Fact]
    public void Services_ShouldNotBeNull()
    {
        Assert.NotNull(_services);
    }

    [Fact]
    public void ShouldResolveAllSingletons()
    {
        Assert.NotNull(_services.GetRequiredService<IBankAccountRepository>());
        Assert.NotNull(_services.GetRequiredService<ICategoryRepository>());
        Assert.NotNull(_services.GetRequiredService<IOperationRepository>());
        
        Assert.NotNull(_services.GetRequiredService<IBankAccountFactory>());
        Assert.NotNull(_services.GetRequiredService<ICategoryFactory>());
        Assert.NotNull(_services.GetRequiredService<IOperationFactory>());
        
        Assert.NotNull(_services.GetRequiredService<IBankAccountFacade>());
        Assert.NotNull(_services.GetRequiredService<ICategoryFacade>());
        Assert.NotNull(_services.GetRequiredService<IOperationFacade>());
        Assert.NotNull(_services.GetRequiredService<IAnalysis>());
        
        Assert.NotNull(_services.GetRequiredService<Menu>());
    }

    [Fact]
    public void ShouldResolveAllTransientCommands()
    {
        var command1 = _services.GetRequiredService<CreateBankAccountCommand>();
        var command2 = _services.GetRequiredService<CreateBankAccountCommand>();

        Assert.NotNull(command1);
        Assert.NotNull(command2);
        Assert.NotSame(command1, command2);
    }

    [Fact]
    public void ShouldResolveSameSingletons()
    {
        var repo1 = _services.GetRequiredService<IBankAccountRepository>();
        var repo2 = _services.GetRequiredService<IBankAccountRepository>();

        Assert.Same(repo1, repo2);

        var facade1 = _services.GetRequiredService<IBankAccountFacade>();
        var facade2 = _services.GetRequiredService<IBankAccountFacade>();

        Assert.Same(facade1, facade2);
    }

    [Fact]
    public void ShouldResolveDifferentInstancesForTransientCommands()
    {
        var cmd1 = _services.GetRequiredService<CreateCategoryCommand>();
        var cmd2 = _services.GetRequiredService<CreateCategoryCommand>();

        Assert.NotSame(cmd1, cmd2);
    }
}