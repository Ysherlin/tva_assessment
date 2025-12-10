using tva_assessment.Application.DTOs;

namespace tva_assessment.Application.Interfaces
{
    /// <summary>
    /// Provides operations for managing accounts.
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Gets an account by code.
        /// </summary>
        Task<AccountDto?> GetByCodeAsync(int code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an account by account number.
        /// </summary>
        Task<AccountDto?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all accounts for a person.
        /// </summary>
        Task<IReadOnlyList<AccountDto>> GetByPersonCodeAsync(int personCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new account.
        /// </summary>
        Task<AccountDto> CreateAsync(AccountDto account, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing account.
        /// </summary>
        Task<AccountDto?> UpdateAsync(AccountDto account, CancellationToken cancellationToken = default);

        /// <summary>
        /// Closes an account.
        /// </summary>
        Task<AccountDto?> CloseAsync(int code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reopens an account.
        /// </summary>
        Task<AccountDto?> ReopenAsync(int code, CancellationToken cancellationToken = default);
    }
}
