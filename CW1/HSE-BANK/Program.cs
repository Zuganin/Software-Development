using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Facades;
using HSE_BANK.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace HSE_BANK;


class Program
{
    static void Main(string[] args)
    {
        var services = DIContainer.Services;

        var menu = services.GetRequiredService<Menu>();
        
        menu.RunMenu();
        
    }
}