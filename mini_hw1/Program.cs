
using hw1.Infrastructure.DIContainer;
using hw1.Services;
using hw1.Infrastructure.Menu;
using Microsoft.Extensions.DependencyInjection;

namespace hw1;

class Program
{
    static void Main(string[] args)
    {
        var servises = DIContainer.ConfigureServices();
        var zoo = servises.GetRequiredService<Zoo>();
        var menu = servises.GetRequiredService<Menu>();
        menu.Show();
    }
    
}