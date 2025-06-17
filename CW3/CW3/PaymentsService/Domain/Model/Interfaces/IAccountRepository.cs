using PaymentsService.Domain.Model.Entities;

namespace PaymentsService.Domain.Model.Interfaces
{
    public interface IAccountRepository
    {
        /// <summary>
        /// Получить аккаунт по идентификатору пользователя.
        /// </summary>
        Task<Account?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Добавить новый аккаунт в репозиторий.
        /// </summary>
        Task AddAsync(Account account, CancellationToken cancellationToken);

        /// <summary>
        /// Обновить существующий аккаунт.
        /// </summary>
        Task UpdateAsync(Account account, CancellationToken cancellationToken);
    }
}