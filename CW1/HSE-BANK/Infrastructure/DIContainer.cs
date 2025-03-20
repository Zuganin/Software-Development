namespace HSE_BANK.Infrastructure;
using Microsoft.Extensions.DependencyInjection;


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