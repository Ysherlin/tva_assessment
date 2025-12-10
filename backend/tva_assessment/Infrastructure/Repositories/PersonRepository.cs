using Microsoft.EntityFrameworkCore;
using tva_assessment.Application.Interfaces;
using tva_assessment.Domain.Entities;
using tva_assessment.Infrastructure.Persistence;

namespace tva_assessment.Infrastructure.Repositories
{
    /// <summary>
    /// Provides Entity Framework-based operations for persons.
    /// </summary>
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Creates a new instance of the person repository.
        /// </summary>
        /// <param name="context">The database context to use.</param>
        public PersonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all persons.
        /// </summary>
        public async Task<IReadOnlyList<Person>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Persons
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets a person by code.
        /// </summary>
        public async Task<Person?> GetByCodeAsync(int code, CancellationToken cancellationToken = default)
        {
            return await _context.Persons
                .Include(p => p.Accounts)
                    .ThenInclude(a => a.Status)
                .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);
        }

        /// <summary>
        /// Gets a person by ID number.
        /// </summary>
        public async Task<Person?> GetByIdNumberAsync(string idNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Persons
                .Include(p => p.Accounts)
                    .ThenInclude(a => a.Status)
                .FirstOrDefaultAsync(p => p.IdNumber == idNumber, cancellationToken);
        }

        /// <summary>
        /// Checks if a person exists by ID number.
        /// </summary>
        public async Task<bool> ExistsByIdNumberAsync(string idNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Persons
                .AnyAsync(p => p.IdNumber == idNumber, cancellationToken);
        }

        /// <summary>
        /// Adds a new person.
        /// </summary>
        public async Task AddAsync(Person person, CancellationToken cancellationToken = default)
        {
            await _context.Persons.AddAsync(person, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates an existing person.
        /// </summary>
        public async Task UpdateAsync(Person person, CancellationToken cancellationToken = default)
        {
            _context.Persons.Update(person);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Deletes an existing person.
        /// </summary>
        public async Task DeleteAsync(Person person, CancellationToken cancellationToken = default)
        {
            _context.Persons.Remove(person);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Searches for persons with paging.
        /// </summary>
        public async Task<IReadOnlyList<Person>> SearchAsync(string? idNumber, string? surname, string? accountNumber, int skip, int take, CancellationToken cancellationToken = default)
        {
            var query = BuildSearchQuery(idNumber, surname, accountNumber);

            query = query
                .Include(p => p.Accounts)
                .OrderBy(p => p.Surname)
                .ThenBy(p => p.Name)
                .ThenBy(p => p.IdNumber);

            return await query
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the number of persons that match the search criteria.
        /// </summary>
        public async Task<int> CountAsync(string? idNumber, string? surname, string? accountNumber, CancellationToken cancellationToken = default)
        {
            var query = BuildSearchQuery(idNumber, surname, accountNumber);
            return await query.CountAsync(cancellationToken);
        }

        /// <summary>
        /// Builds the base query for person searching.
        /// </summary>
        private IQueryable<Person> BuildSearchQuery(string? idNumber, string? surname, string? accountNumber)
        {
            var query = _context.Persons.AsQueryable();

            if (!string.IsNullOrWhiteSpace(idNumber))
            {
                query = query.Where(p => p.IdNumber == idNumber);
            }

            if (!string.IsNullOrWhiteSpace(surname))
            {
                query = query.Where(p => p.Surname != null && p.Surname.Contains(surname));
            }

            if (!string.IsNullOrWhiteSpace(accountNumber))
            {
                query = query.Where(p => p.Accounts.Any(a => a.AccountNumber == accountNumber));
            }

            return query;
        }
    }
}
