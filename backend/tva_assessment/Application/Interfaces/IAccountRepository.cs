using tva_assessment.Domain.Entities;

namespace tva_assessment.Application.Interfaces
{
    /// <summary>
    /// Provides data access operations for accounts.
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// Gets an account by its identifier.
        /// </summary>
        Task<Account?> GetByCodeAsync(int code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an account by its account number.
        /// </summary>
        Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all accounts for a person.
        /// </summary>
        Task<IReadOnlyList<Account>> GetByPersonCodeAsync(int personCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new account.
        /// </summary>
        Task AddAsync(Account account, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing account.
        /// </summary>
        Task UpdateAsync(Account account, CancellationToken cancellationToken = default);
    }
}
