namespace hw1.Infrastructure.Menu;
using hw1.Services;
using hw1.Models.Animals;
using hw1.Models.Things;

using Spectre.Console;


public class Menu
{
    private readonly Zoo _zoo;

    public Menu(Zoo zoo)
    {
        _zoo = zoo;
    }

    public void Show()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText("Московский Зоопарк").Centered().Color(Color.Green));

        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите действие:")
                    .PageSize(10)
                    .AddChoices(
                        "Добавить животное",
                        "Показать всех животных",
                        "Общее количество еды",
                        "Животные для контактного зоопарка",
                        "Добавить предмет на склад",
                        "Инвентаризация вещей",
                        "Выход"
                    )
            );

            switch (choice)
            {
                case "Добавить животное":
                    AddAnimal();
                    break;
                case "Показать всех животных":
                    ShowAllAnimals();
                    break;
                case "Общее количество еды":
                    ShowTotalFood();
                    break;
                case "Животные для контактного зоопарка":
                    ShowContactZooAnimals();
                    break;
                case "Инвентаризация вещей":
                    ShowInventory();
                    break;
                case  "Добавить предмет на склад":
                    AddThing();
                    break;
                case "Выход":
                    return;
            }

            AnsiConsole.WriteLine("Нажмите Enter для продолжения...");
            Console.ReadLine();
        }
    }

    private void AddAnimal()
    {
        AnsiConsole.Clear();
        var animalType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Выберите тип животного:")
                .AddChoices("Tiger", "Rabbit", "Monkey", "Wolf")
        );

        var name = AnsiConsole.Ask<string>("Введите имя животного:");
        var food = AnsiConsole.Ask<int>("Введите количество еды (кг/день):");
        var number = AnsiConsole.Ask<int>("Введите инвентаризационный номер:");

        Animal animal = animalType switch
        {
            "Tiger" => new Tiger(name, food, number ) ,
            "Rabbit" => new Rabbit(name, food, number ) ,
            "Monkey" => new Monkey(name, food, number ),
            "Wolf" => new Wolf(name, food, number ),
            _ => throw new ArgumentException("Неизвестный тип животного")
        };

        _zoo.AddAnimal(animal);
        AnsiConsole.MarkupLine($"[green]{animal.Name} добавлен![/]");
    }

    private void AddThing()
    {
        AnsiConsole.Clear();
        var thingType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Выберите вещь:")
                .AddChoices("Table", "Computer")
        );
        
        var number = AnsiConsole.Ask<int>("Введите инвентарный номер:");

        Thing thing = thingType switch
        {
            "Table" => new Models.Things.Table(number),
            "Computer" => new Computer(number),
            _ => throw new ArgumentException("Неизвестный тип предмета")
        };

        _zoo.AddThing(thing);
        AnsiConsole.MarkupLine($"[green] Предмет добавлен![/]");
    }
    
    private void ShowAllAnimals()
    {
        AnsiConsole.Clear();
        var table = new Spectre.Console.Table()
            .Border(TableBorder.Rounded)
            .Title("Список животных")
            .AddColumn("Тип")
            .AddColumn("Имя")
            .AddColumn("Номер")
            .AddColumn("Еда (кг/день)");

        foreach (var animal in _zoo.GetAnimals())
        {
            table.AddRow(
                animal.GetType().Name,
                animal.Name,
                animal.Number.ToString(),
                animal.Food.ToString()
            );
        }

        AnsiConsole.Write(table);
    }
    
    private void ShowTotalFood()
    {
        AnsiConsole.Clear();
        var totalFood = _zoo.CalculateTotalFood();
        AnsiConsole.MarkupLine($"[bold]Общее количество еды:[/] [yellow]{totalFood} кг/день[/]");
    }

    private void ShowContactZooAnimals()
    {
        AnsiConsole.Clear();
        var animals = _zoo.GetAnimals()
            .OfType<Herbo>()
            .Where(m => m.CanBeInContactZoo());

        var panel = new Panel(new Text(string.Join("\n", animals.Select(a => a.ToString()))))
            .Header("Животные для контактного зоопарка")
            .Border(BoxBorder.Rounded);

        AnsiConsole.Write(panel);
    }
    private void ShowInventory()
    {
        AnsiConsole.Clear();
        var table = new Spectre.Console.Table()
            .Border(TableBorder.Rounded)
            .Title("Инвентаризация")
            .AddColumn("[bold]Тип[/]")
            .AddColumn("[bold]Номер[/]");

        foreach (var thing in _zoo.GetThings())
        {
            table.AddRow(
                thing.GetType().Name,
                thing.Number.ToString()
            );
        }

        AnsiConsole.Write(table);
    }
    
    
}