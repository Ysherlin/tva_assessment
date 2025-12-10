using Microsoft.EntityFrameworkCore;
using tva_assessment.Application.Interfaces;
using tva_assessment.Domain.Entities;
using tva_assessment.Infrastructure.Persistence;

namespace tva_assessment.Infrastructure.Repositories
{
    /// <summary>
    /// Provides Entity Framework-based operations for accounts.
    /// </summary>
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Creates a new instance of the account repository.
        /// </summary>
        /// <param name="context">The database context to use.</param>
        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets an account by code.
        /// </summary>
        public async Task<Account?> GetByCodeAsync(int code, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts
                .Include(a => a.Person)
                .Include(a => a.Status)
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.Code == code, cancellationToken);
        }

        /// <summary>
        /// Gets an account by account number.
        /// </summary>
        public async Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts
                .Include(a => a.Person)
                .Include(a => a.Status)
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber, cancellationToken);
        }

        /// <summary>
        /// Gets all accounts for a person.
        /// </summary>
        public async Task<IReadOnlyList<Account>> GetByPersonCodeAsync(int personCode, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts
                .Where(a => a.PersonCode == personCode)
                .Include(a => a.Status)
                .Include(a => a.Transactions)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Adds a new account.
        /// </summary>
        public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
        {
            await _context.Accounts.AddAsync(account, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates an existing account.
        /// </summary>
        public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
