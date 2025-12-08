using Microsoft.EntityFrameworkCore;
using tva_assessment.Application.Interfaces;
using tva_assessment.Domain.Entities;
using tva_assessment.Infrastructure.Persistence;

namespace tva_assessment.Infrastructure.Repositories
{
    /// <summary>
    /// Provides Entity Framework-based operations for transactions.
    /// </summary>
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Creates a new instance of the transaction repository.
        /// </summary>
        /// <param name="context">The database context to use.</param>
        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a transaction by code.
        /// </summary>
        public async Task<Transaction?> GetByCodeAsync(int code, CancellationToken cancellationToken = default)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .FirstOrDefaultAsync(t => t.Code == code, cancellationToken);
        }

        /// <summary>
        /// Gets all transactions for an account.
        /// </summary>
        public async Task<IReadOnlyList<Transaction>> GetByAccountCodeAsync(int accountCode, CancellationToken cancellationToken = default)
        {
            return await _context.Transactions
                .Where(t => t.AccountCode == accountCode)
                .OrderBy(t => t.TransactionDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Adds a new transaction.
        /// </summary>
        public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
        {
            await _context.Transactions.AddAsync(transaction, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates an existing transaction.
        /// </summary>
        public async Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
