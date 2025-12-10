using System.Linq;
using tva_assessment.Application.DTOs;
using tva_assessment.Application.Interfaces;
using tva_assessment.Domain.Entities;

namespace tva_assessment.Application.Services
{
    /// <summary>
    /// Provides transaction-related business operations.
    /// </summary>
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;

        /// <summary>
        /// Creates a new instance of the transaction service.
        /// </summary>
        /// <param name="transactionRepository">The transaction repository to use.</param>
        /// <param name="accountRepository">The account repository to use.</param>
        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }

        /// <summary>
        /// Gets a transaction by code.
        /// </summary>
        public async Task<TransactionDto?> GetByCodeAsync(int code, CancellationToken cancellationToken = default)
        {
            var transaction = await _transactionRepository.GetByCodeAsync(code, cancellationToken);
            return transaction is null ? null : MapToDto(transaction);
        }

        /// <summary>
        /// Gets all transactions for an account.
        /// </summary>
        public async Task<IReadOnlyList<TransactionDto>> GetByAccountCodeAsync(int accountCode, CancellationToken cancellationToken = default)
        {
            var transactions = await _transactionRepository.GetByAccountCodeAsync(accountCode, cancellationToken);
            return transactions
                .Select(MapToDto)
                .ToList();
        }

        /// <summary>
        /// Creates a new transaction.
        /// </summary>
        public async Task<TransactionDto> CreateAsync(TransactionDto transactionDto, CancellationToken cancellationToken = default)
        {
            var account = await _accountRepository.GetByCodeAsync(transactionDto.AccountCode, cancellationToken);
            if (account is null)
            {
                throw new InvalidOperationException("The account does not exist.");
            }

            ValidateTransaction(transactionDto);

            var transaction = new Transaction
            {
                AccountCode = transactionDto.AccountCode,
                TransactionDate = transactionDto.TransactionDate,
                CaptureDate = DateTime.UtcNow,
                Amount = transactionDto.Amount,
                Description = transactionDto.Description
            };

            await _transactionRepository.AddAsync(transaction, cancellationToken);

            await UpdateAccountBalanceAsync(transaction.AccountCode, cancellationToken);

            return MapToDto(transaction);
        }

        /// <summary>
        /// Updates an existing transaction.
        /// </summary>
        public async Task<TransactionDto?> UpdateAsync(TransactionDto transactionDto, CancellationToken cancellationToken = default)
        {
            var existing = await _transactionRepository.GetByCodeAsync(transactionDto.Code, cancellationToken);
            if (existing is null)
            {
                return null;
            }

            if (existing.AccountCode != transactionDto.AccountCode)
            {
                throw new InvalidOperationException("Changing the account of a transaction is not allowed.");
            }

            ValidateTransaction(transactionDto);

            existing.TransactionDate = transactionDto.TransactionDate;
            existing.Amount = transactionDto.Amount;
            existing.Description = transactionDto.Description;
            existing.CaptureDate = DateTime.UtcNow;

            await _transactionRepository.UpdateAsync(existing, cancellationToken);

            await UpdateAccountBalanceAsync(existing.AccountCode, cancellationToken);

            return MapToDto(existing);
        }

        /// <summary>
        /// Validates basic transaction rules.
        /// </summary>
        /// <param name="transactionDto">The transaction to validate.</param>
        /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
        private static void ValidateTransaction(TransactionDto transactionDto)
        {
            if (transactionDto.Amount == 0)
            {
                throw new InvalidOperationException("The transaction amount cannot be zero.");
            }

            if (transactionDto.TransactionDate > DateTime.UtcNow)
            {
                throw new InvalidOperationException("The transaction date cannot be in the future.");
            }
        }

        /// <summary>
        /// Recalculates and updates the account balance.
        /// </summary>
        private async Task UpdateAccountBalanceAsync(int accountCode, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByCodeAsync(accountCode, cancellationToken);
            if (account is null)
            {
                return;
            }

            var transactions = await _transactionRepository.GetByAccountCodeAsync(accountCode, cancellationToken);
            account.OutstandingBalance = transactions.Sum(t => t.Amount);

            await _accountRepository.UpdateAsync(account, cancellationToken);
        }

        /// <summary>
        /// Maps a transaction entity to a transaction DTO.
        /// </summary>
        private static TransactionDto MapToDto(Transaction transaction)
        {
            return new TransactionDto
            {
                Code = transaction.Code,
                AccountCode = transaction.AccountCode,
                TransactionDate = transaction.TransactionDate,
                CaptureDate = transaction.CaptureDate,
                Amount = transaction.Amount,
                Description = transaction.Description
            };
        }
    }
}
