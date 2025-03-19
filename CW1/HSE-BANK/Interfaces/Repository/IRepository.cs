namespace HSE_BANK.Interfaces.Repository;

public interface IRepository<T>
{
    T GetById(Guid id);
    void Add(T category);
    void Update(T obj);
    void Delete(Guid id);
    IEnumerable<T> GetAll();
}