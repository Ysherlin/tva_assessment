using tva_assessment.Application.DTOs;

namespace tva_assessment.Application.Interfaces
{
    /// <summary>
    /// Provides operations for managing persons.
    /// </summary>
    public interface IPersonService
    {
        /// <summary>
        /// Gets all persons.
        /// </summary>
        Task<IReadOnlyList<PersonDto>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a person by code.
        /// </summary>
        Task<PersonDto?> GetByCodeAsync(int code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new person.
        /// </summary>
        Task<PersonDto> CreateAsync(PersonDto person, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing person.
        /// </summary>
        Task<PersonDto?> UpdateAsync(PersonDto person, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an existing person.
        /// </summary>
        Task<bool> DeleteAsync(int code, CancellationToken cancellationToken = default);
    }
}
