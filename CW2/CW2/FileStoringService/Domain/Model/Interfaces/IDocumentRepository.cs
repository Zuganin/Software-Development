using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStoringService.Domain.Model.Entities;
using FileStoringService.Entities;

namespace FileStoringService.Domain.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для работы с документами
    /// </summary>
    public interface IDocumentRepository
    {
        /// <summary>
        /// Добавляет новый документ в репозиторий
        /// </summary>
        /// <param name="document">Документ для добавления</param>
        /// <returns>Добавленный документ</returns>
        Task<Document> AddAsync(Document document);

        /// <summary>
        /// Получает документ по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор документа</param>
        /// <returns>Документ или null, если документ не найден</returns>
        Task<Document> GetByIdAsync(Guid id);

        /// <summary>
        /// Получает документ по его хешу
        /// </summary>
        /// <param name="hash">Хеш документа</param>
        /// <returns>Документ или null, если документ с таким хешем не найден</returns>
        Task<Document> GetByHashAsync(string hash);

        /// <summary>
        /// Проверяет существование документа с указанным хешем
        /// </summary>
        /// <param name="hash">Хеш для проверки</param>
        /// <returns>true, если документ с таким хешем существует, иначе false</returns>
        Task<bool> ExistsByHashAsync(string hash);

        /// <summary>
        /// Получает список всех документов
        /// </summary>
        /// <returns>Список документов</returns>
        Task<IEnumerable<Document>> GetAllAsync();

        /// <summary>
        /// Удаляет документ из репозитория
        /// </summary>
        /// <param name="id">Идентификатор документа для удаления</param>
        /// <returns>true, если документ был удален, иначе false</returns>
        Task<bool> DeleteAsync(Guid id);
    }
}
