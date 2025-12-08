using tva_assessment.Domain.Entities;

namespace tva_assessment.Application.Interfaces
{
    /// <summary>
    /// Provides data access operations for transactions.
    /// </summary>
    public interface ITransactionRepository
    {
        /// <summary>
        /// Gets a transaction by its identifier.
        /// </summary>
        Task<Transaction?> GetByCodeAsync(int code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all transactions for an account.
        /// </summary>
        Task<IReadOnlyList<Transaction>> GetByAccountCodeAsync(int accountCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new transaction.
        /// </summary>
        Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing transaction.
        /// </summary>
        Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default);
    }
}
