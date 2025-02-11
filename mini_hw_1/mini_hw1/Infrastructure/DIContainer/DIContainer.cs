using hw1.Models.Interfaces;
using hw1.Services;
using hw1.Infrastructure.Menu;
using Microsoft.Extensions.DependencyInjection;

namespace hw1.Infrastructure.DIContainer;

public class DIContainer
{
    public static ServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton<IVetclinic,Vetclinic>() // Регистрация ветеринарной клиники
            .AddSingleton<Zoo>() // Регистрация зоопарка
            .AddSingleton<Menu.Menu>()
            .BuildServiceProvider();
    }
}