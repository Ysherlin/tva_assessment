using tva_assessment.Domain.Entities;

namespace tva_assessment.Application.Interfaces
{
    /// <summary>
    /// Provides data access operations for persons.
    /// </summary>
    public interface IPersonRepository
    {
        /// <summary>
        /// Gets all persons.
        /// </summary>
        Task<IReadOnlyList<Person>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a person by their identifier.
        /// </summary>
        Task<Person?> GetByCodeAsync(int code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a person by their ID number.
        /// </summary>
        Task<Person?> GetByIdNumberAsync(string idNumber, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a person exists with the given ID number.
        /// </summary>
        Task<bool> ExistsByIdNumberAsync(string idNumber, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new person.
        /// </summary>
        Task AddAsync(Person person, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing person.
        /// </summary>
        Task UpdateAsync(Person person, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an existing person.
        /// </summary>
        Task DeleteAsync(Person person, CancellationToken cancellationToken = default);
    }
}
