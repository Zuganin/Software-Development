using FileStoringService.Domain.Model.Entities;
using FileStoringService.Entities;

namespace FileStoringService.Application.Interfaces;

public interface IFileRepository
{
    // Поиск по хешу
    Task<Document?> GetByHashAsync(string hash, CancellationToken ct = default);

    // Получение по идентификатору
    Task<Document?> GetByIdAsync(Guid id, CancellationToken ct = default);

    // Список всех
    Task<IEnumerable<Document>> ListAsync(CancellationToken ct = default);

    // Добавление
    Task AddAsync(Document file, CancellationToken ct = default);

    // Удаление
    Task DeleteAsync(Document file, CancellationToken ct = default);
}