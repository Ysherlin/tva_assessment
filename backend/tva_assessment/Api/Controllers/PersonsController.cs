using Microsoft.AspNetCore.Mvc;
using tva_assessment.Application.DTOs;
using tva_assessment.Application.Interfaces;

namespace tva_assessment.Api.Controllers
{
    /// <summary>
    /// Provides API endpoints for managing persons.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly IPersonService _personService;

        /// <summary>
        /// Creates a new instance of the persons controller.
        /// </summary>
        /// <param name="personService">The person service to use.</param>
        public PersonsController(IPersonService personService)
        {
            _personService = personService;
        }

        /// <summary>
        /// Gets all persons.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<PersonDto>>> GetAll(CancellationToken cancellationToken)
        {
            var persons = await _personService.GetAllAsync(cancellationToken);
            return Ok(persons);
        }

        /// <summary>
        /// Gets a person by code.
        /// </summary>
        [HttpGet("{code:int}")]
        public async Task<ActionResult<PersonDto>> GetByCode(int code, CancellationToken cancellationToken)
        {
            var person = await _personService.GetByCodeAsync(code, cancellationToken);
            if (person is null)
            {
                return NotFound();
            }

            return Ok(person);
        }

        /// <summary>
        /// Creates a new person.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PersonDto>> Create(PersonDto personDto, CancellationToken cancellationToken)
        {
            var created = await _personService.CreateAsync(personDto, cancellationToken);
            return CreatedAtAction(nameof(GetByCode), new { code = created.Code }, created);
        }

        /// <summary>
        /// Updates an existing person.
        /// </summary>
        [HttpPut("{code:int}")]
        public async Task<ActionResult<PersonDto>> Update(int code, PersonDto personDto, CancellationToken cancellationToken)
        {
            if (code != personDto.Code)
            {
                return BadRequest("The route code and body code must match.");
            }

            var updated = await _personService.UpdateAsync(personDto, cancellationToken);
            if (updated is null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        /// <summary>
        /// Deletes an existing person.
        /// </summary>
        [HttpDelete("{code:int}")]
        public async Task<IActionResult> Delete(int code, CancellationToken cancellationToken)
        {
            var deleted = await _personService.DeleteAsync(code, cancellationToken);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
