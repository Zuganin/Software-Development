
using hw1.Models.Interfaces;

namespace hw1.Models.Animals;

public class Animal : IAlive, IInventory
{
    public bool IsHealthy { get; set; } = true;
    public int Food { get; set; }
    public int Number { get; set; }
    public string Name { get; }
    
    protected Animal(string name, int food, int number)
    {
        Name = name;
        Food = food;
        Number = number;
    }

    public override string ToString()
    {
        string info = $"Животное: {GetType().Name}, под номером: {Number}, зовут: {Name}, потребляет пищи в день: {Food} кг";

        // Если животное травоядное, добавляем уровень доброты
        if (this is Herbo herbivore)
        {
            info += $", уровень доброты: {herbivore.Kindness}";
            info += $", может быть в контактном зоопарке: {(herbivore.Kindness > 5 ? "Да" : "Нет")}";
        }

        return info;
    }
}