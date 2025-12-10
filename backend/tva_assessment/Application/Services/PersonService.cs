using tva_assessment.Application.DTOs;
using tva_assessment.Application.Interfaces;
using tva_assessment.Domain.Entities;

namespace tva_assessment.Application.Services
{
    /// <summary>
    /// Provides person-related business operations.
    /// </summary>
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;

        /// <summary>
        /// Creates a new instance of the person service.
        /// </summary>
        /// <param name="personRepository">The person repository to use.</param>
        public PersonService(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        /// <summary>
        /// Gets all persons.
        /// </summary>
        public async Task<IReadOnlyList<PersonDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var persons = await _personRepository.GetAllAsync(cancellationToken);
            return persons
                .Select(MapToDto)
                .ToList();
        }

        /// <summary>
        /// Gets a person by code.
        /// </summary>
        public async Task<PersonDto?> GetByCodeAsync(int code, CancellationToken cancellationToken = default)
        {
            var person = await _personRepository.GetByCodeAsync(code, cancellationToken);
            return person is null ? null : MapToDto(person);
        }

        /// <summary>
        /// Creates a new person.
        /// </summary>
        public async Task<PersonDto> CreateAsync(PersonDto personDto, CancellationToken cancellationToken = default)
        {
            // Simple business rule: ID number must be unique.
            var exists = await _personRepository.ExistsByIdNumberAsync(personDto.IdNumber, cancellationToken);
            if (exists)
            {
                throw new InvalidOperationException("A person with the same ID number already exists.");
            }

            var person = new Person
            {
                IdNumber = personDto.IdNumber,
                Name = personDto.Name,
                Surname = personDto.Surname
            };

            await _personRepository.AddAsync(person, cancellationToken);

            return MapToDto(person);
        }

        /// <summary>
        /// Updates an existing person.
        /// </summary>
        public async Task<PersonDto?> UpdateAsync(PersonDto personDto, CancellationToken cancellationToken = default)
        {
            var existing = await _personRepository.GetByCodeAsync(personDto.Code, cancellationToken);
            if (existing is null)
            {
                return null;
            }
            if (!string.Equals(existing.IdNumber, personDto.IdNumber, StringComparison.OrdinalIgnoreCase))
            {
                var exists = await _personRepository.ExistsByIdNumberAsync(personDto.IdNumber, cancellationToken);
                if (exists)
                {
                    throw new InvalidOperationException("A person with the same ID number already exists.");
                }

                existing.IdNumber = personDto.IdNumber;
            }

            existing.Name = personDto.Name;
            existing.Surname = personDto.Surname;

            await _personRepository.UpdateAsync(existing, cancellationToken);

            return MapToDto(existing);
        }

        /// <summary>
        /// Deletes an existing person if all account rules are satisfied.
        /// </summary>
        public async Task<bool> DeleteAsync(int code, CancellationToken cancellationToken = default)
        {
            var existing = await _personRepository.GetByCodeAsync(code, cancellationToken);
            if (existing is null)
            {
                return false;
            }

            if (!existing.Accounts.Any())
            {
                await _personRepository.DeleteAsync(existing, cancellationToken);
                return true;
            }

            var hasAccountWithBalance = existing.Accounts.Any(a => a.OutstandingBalance != 0m);
            if (hasAccountWithBalance)
            {
                throw new InvalidOperationException( "The person cannot be deleted because one or more accounts have an outstanding balance.");
            }

            await _personRepository.DeleteAsync(existing, cancellationToken);
            return true;
        }

        /// <summary>
        /// Searches for persons with paging.
        /// </summary>
        public async Task<PagedResultDto<PersonDto>> SearchAsync( string? idNumber, string? surname, string? accountNumber, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1)
            {
                throw new InvalidOperationException("Page number must be at least 1.");
            }

            const int maxPageSize = 10;
            if (pageSize <= 0 || pageSize > maxPageSize)
            {
                pageSize = maxPageSize;
            }

            var skip = (pageNumber - 1) * pageSize;

            var totalCount = await _personRepository.CountAsync(idNumber, surname, accountNumber, cancellationToken);
            var persons = await _personRepository.SearchAsync(idNumber, surname, accountNumber, skip, pageSize, cancellationToken);

            var items = persons
                .Select(MapToDto)
                .ToList();

            var totalPages = totalCount == 0
                ? 0
                : (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PagedResultDto<PersonDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        /// <summary>
        /// Maps a person entity to a person DTO.
        /// </summary>
        private static PersonDto MapToDto(Person person)
        {
            return new PersonDto
            {
                Code = person.Code,
                IdNumber = person.IdNumber,
                Name = person.Name,
                Surname = person.Surname
            };
        }
    }
}
