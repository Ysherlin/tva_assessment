using Microsoft.AspNetCore.Mvc;
using tva_assessment.Application.DTOs;
using tva_assessment.Application.Interfaces;

namespace tva_assessment.Api.Controllers
{
    /// <summary>
    /// Provides API endpoints for managing accounts.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        /// <summary>
        /// Creates a new instance of the accounts controller.
        /// </summary>
        /// <param name="accountService">The account service to use.</param>
        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Gets an account by code.
        /// </summary>
        [HttpGet("{code:int}")]
        public async Task<ActionResult<AccountDto>> GetByCode(int code, CancellationToken cancellationToken)
        {
            var account = await _accountService.GetByCodeAsync(code, cancellationToken);
            if (account is null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        /// <summary>
        /// Gets an account by account number.
        /// </summary>
        [HttpGet("by-number/{accountNumber}")]
        public async Task<ActionResult<AccountDto>> GetByAccountNumber(string accountNumber, CancellationToken cancellationToken)
        {
            var account = await _accountService.GetByAccountNumberAsync(accountNumber, cancellationToken);
            if (account is null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        /// <summary>
        /// Gets all accounts for a person.
        /// </summary>
        [HttpGet("by-person/{personCode:int}")]
        public async Task<ActionResult<IReadOnlyList<AccountDto>>> GetByPerson(int personCode, CancellationToken cancellationToken)
        {
            var accounts = await _accountService.GetByPersonCodeAsync(personCode, cancellationToken);
            return Ok(accounts);
        }

        /// <summary>
        /// Creates a new account.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AccountDto>> Create(AccountDto accountDto, CancellationToken cancellationToken)
        {
            var created = await _accountService.CreateAsync(accountDto, cancellationToken);
            return CreatedAtAction(nameof(GetByCode), new { code = created.Code }, created);
        }

        /// <summary>
        /// Updates an existing account.
        /// </summary>
        [HttpPut("{code:int}")]
        public async Task<ActionResult<AccountDto>> Update(int code, AccountDto accountDto, CancellationToken cancellationToken)
        {
            if (code != accountDto.Code)
            {
                return BadRequest("The route code and body code must match.");
            }

            var updated = await _accountService.UpdateAsync(accountDto, cancellationToken);
            if (updated is null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        /// <summary>
        /// Closes an account.
        /// </summary>
        [HttpPost("{code:int}/close")]
        public async Task<ActionResult<AccountDto>> Close(int code, CancellationToken cancellationToken)
        {
            var result = await _accountService.CloseAsync(code, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Reopens an account.
        /// </summary>
        [HttpPost("{code:int}/reopen")]
        public async Task<ActionResult<AccountDto>> Reopen(int code, CancellationToken cancellationToken)
        {
            var result = await _accountService.ReopenAsync(code, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
    }
}
