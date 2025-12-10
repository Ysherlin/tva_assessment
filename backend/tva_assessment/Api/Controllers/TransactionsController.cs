using Microsoft.AspNetCore.Mvc;
using tva_assessment.Application.DTOs;
using tva_assessment.Application.Interfaces;

namespace tva_assessment.Controllers
{
    /// <summary>
    /// Provides API endpoints for managing transactions.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        /// <summary>
        /// Creates a new instance of the transactions controller.
        /// </summary>
        /// <param name="transactionService">The transaction service to use.</param>
        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Gets a transaction by code.
        /// </summary>
        [HttpGet("{code:int}")]
        public async Task<ActionResult<TransactionDto>> GetByCode(int code, CancellationToken cancellationToken)
        {
            var transaction = await _transactionService.GetByCodeAsync(code, cancellationToken);
            if (transaction is null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        /// <summary>
        /// Gets all transactions for an account.
        /// </summary>
        [HttpGet("by-account/{accountCode:int}")]
        public async Task<ActionResult<IReadOnlyList<TransactionDto>>> GetByAccount(int accountCode, CancellationToken cancellationToken)
        {
            var transactions = await _transactionService.GetByAccountCodeAsync(accountCode, cancellationToken);
            return Ok(transactions);
        }

        /// <summary>
        /// Creates a new transaction.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TransactionDto>> Create(TransactionDto transactionDto, CancellationToken cancellationToken)
        {
            var created = await _transactionService.CreateAsync(transactionDto, cancellationToken);
            return CreatedAtAction(nameof(GetByCode), new { code = created.Code }, created);
        }

        /// <summary>
        /// Updates an existing transaction.
        /// </summary>
        [HttpPut("{code:int}")]
        public async Task<ActionResult<TransactionDto>> Update(int code, TransactionDto transactionDto, CancellationToken cancellationToken)
        {
            if (code != transactionDto.Code)
            {
                return BadRequest("The route code and body code must match.");
            }

            var updated = await _transactionService.UpdateAsync(transactionDto, cancellationToken);
            if (updated is null)
            {
                return NotFound();
            }

            return Ok(updated);
        }
    }
}
