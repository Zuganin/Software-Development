using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Facades;
using HSE_BANK.Interfaces.Command;

namespace HSE_BANK.Commands;

public class CreateCategoryCommand : ICommands
{
    private readonly ICategoryFacade _categoryFacade;
    private string _name;
    private CategoryType _type;
    
    public CreateCategoryCommand(ICategoryFacade categoryFacade)
    {
        _categoryFacade = categoryFacade;
    }
    
    public void Create(string name, CategoryType type)
    {
        _name = name;
        _type = type;
    }
    public void Execute()
    {
        _categoryFacade.CreateCategory(_name, _type);
    }
}