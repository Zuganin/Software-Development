namespace HSE_BANK.Interfaces.Repository;

public interface IRepository<T>
{
    T GetById(Guid id);
    void Add(T operation);
    void Update(T operation);
    void Delete(Guid id);
    IEnumerable<T> GetAll();
}