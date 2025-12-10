using System.Linq;
using tva_assessment.Application.DTOs;
using tva_assessment.Application.Interfaces;
using tva_assessment.Domain.Entities;

namespace tva_assessment.Application.Services
{
    /// <summary>
    /// Provides account-related business operations.
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPersonRepository _personRepository;

        /// <summary>
        /// Creates a new instance of the account service.
        /// </summary>
        /// <param name="accountRepository">The account repository to use.</param>
        /// <param name="personRepository">The person repository to use.</param>
        public AccountService(IAccountRepository accountRepository, IPersonRepository personRepository)
        {
            _accountRepository = accountRepository;
            _personRepository = personRepository;
        }

        /// <summary>
        /// Gets an account by code.
        /// </summary>
        public async Task<AccountDto?> GetByCodeAsync(int code, CancellationToken cancellationToken = default)
        {
            var account = await _accountRepository.GetByCodeAsync(code, cancellationToken);
            return account is null ? null : MapToDto(account);
        }

        /// <summary>
        /// Gets an account by account number.
        /// </summary>
        public async Task<AccountDto?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default)
        {
            var account = await _accountRepository.GetByAccountNumberAsync(accountNumber, cancellationToken);
            return account is null ? null : MapToDto(account);
        }

        /// <summary>
        /// Gets all accounts for a person.
        /// </summary>
        public async Task<IReadOnlyList<AccountDto>> GetByPersonCodeAsync(int personCode, CancellationToken cancellationToken = default)
        {
            var accounts = await _accountRepository.GetByPersonCodeAsync(personCode, cancellationToken);
            return accounts
                .Select(MapToDto)
                .ToList();
        }

        /// <summary>
        /// Creates a new account.
        /// </summary>
        public async Task<AccountDto> CreateAsync(AccountDto accountDto, CancellationToken cancellationToken = default)
        {
            var person = await _personRepository.GetByCodeAsync(accountDto.PersonCode, cancellationToken);
            if (person is null)
            {
                throw new InvalidOperationException("The person does not exist.");
            }

            var existingAccount = await _accountRepository.GetByAccountNumberAsync(accountDto.AccountNumber, cancellationToken);
            if (existingAccount is not null)
            {
                throw new InvalidOperationException("An account with the same account number already exists.");
            }

            var account = new Account
            {
                PersonCode = accountDto.PersonCode,
                AccountNumber = accountDto.AccountNumber,
                OutstandingBalance = 0m,
                Status = new AccountStatus
                {
                    IsClosed = false
                }
            };

            await _accountRepository.AddAsync(account, cancellationToken);

            return MapToDto(account);
        }

        /// <summary>
        /// Updates an existing account.
        /// </summary>
        public async Task<AccountDto?> UpdateAsync(AccountDto accountDto, CancellationToken cancellationToken = default)
        {
            var existing = await _accountRepository.GetByCodeAsync(accountDto.Code, cancellationToken);
            if (existing is null)
            {
                return null;
            }

            if (existing.PersonCode != accountDto.PersonCode)
            {
                var person = await _personRepository.GetByCodeAsync(accountDto.PersonCode, cancellationToken);
                if (person is null)
                {
                    throw new InvalidOperationException("The person does not exist.");
                }

                existing.PersonCode = accountDto.PersonCode;
            }

            if (!string.Equals(existing.AccountNumber, accountDto.AccountNumber, StringComparison.OrdinalIgnoreCase))
            {
                var accountWithNumber = await _accountRepository.GetByAccountNumberAsync(accountDto.AccountNumber, cancellationToken);
                if (accountWithNumber is not null && accountWithNumber.Code != existing.Code)
                {
                    throw new InvalidOperationException("An account with the same account number already exists.");
                }

                existing.AccountNumber = accountDto.AccountNumber;
            }

            await _accountRepository.UpdateAsync(existing, cancellationToken);

            return MapToDto(existing);
        }

        /// <summary>
        /// Closes an account if the balance is zero.
        /// </summary>
        public async Task<AccountDto?> CloseAsync(int code, CancellationToken cancellationToken = default)
        {
            var account = await _accountRepository.GetByCodeAsync(code, cancellationToken);
            if (account is null)
            {
                return null;
            }

            if (account.OutstandingBalance != 0m)
            {
                throw new InvalidOperationException("The account cannot be closed because the balance is not zero.");
            }

            if (account.Status?.IsClosed == true)
            {
                throw new InvalidOperationException("The account is already closed.");
            }

            account.Status ??= new AccountStatus
            {
                AccountCode = account.Code
            };

            account.Status.IsClosed = true;

            await _accountRepository.UpdateAsync(account, cancellationToken);

            return MapToDto(account);
        }

        /// <summary>
        /// Reopens a closed account.
        /// </summary>
        public async Task<AccountDto?> ReopenAsync(int code, CancellationToken cancellationToken = default)
        {
            var account = await _accountRepository.GetByCodeAsync(code, cancellationToken);
            if (account is null)
            {
                return null;
            }

            if (account.Status?.IsClosed != true)
            {
                throw new InvalidOperationException("The account is not closed.");
            }

            account.Status.IsClosed = false;

            await _accountRepository.UpdateAsync(account, cancellationToken);

            return MapToDto(account);
        }

        /// <summary>
        /// Maps an account entity to an account DTO.
        /// </summary>
        private static AccountDto MapToDto(Account account)
        {
            return new AccountDto
            {
                Code = account.Code,
                PersonCode = account.PersonCode,
                AccountNumber = account.AccountNumber,
                OutstandingBalance = account.OutstandingBalance
            };
        }
    }
}
