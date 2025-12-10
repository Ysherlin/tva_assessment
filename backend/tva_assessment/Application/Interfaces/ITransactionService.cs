using tva_assessment.Application.DTOs;

namespace tva_assessment.Application.Interfaces
{
    /// <summary>
    /// Provides operations for managing transactions.
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        /// Gets a transaction by code.
        /// </summary>
        Task<TransactionDto?> GetByCodeAsync(int code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all transactions for an account.
        /// </summary>
        Task<IReadOnlyList<TransactionDto>> GetByAccountCodeAsync(int accountCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new transaction.
        /// </summary>
        Task<TransactionDto> CreateAsync(TransactionDto transaction, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing transaction.
        /// </summary>
        Task<TransactionDto?> UpdateAsync(TransactionDto transaction, CancellationToken cancellationToken = default);
    }
}
