using hw1.Models.Interfaces;

namespace hw1.Models.Things;

public class Thing : IInventory
{
    public int Number { get; set; }
    
    
    protected Thing(int number)
    {
        Number = number;
    }

    public override string ToString()
    {
        return $"Вещь: {GetType().Name} Инвентарный номер: {Number}";
    }
}