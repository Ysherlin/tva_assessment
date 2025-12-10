using System.ComponentModel.DataAnnotations;

namespace tva_assessment.Application.DTOs
{
    /// <summary>
    /// Represents a person returned by the API.
    /// </summary>
    public class PersonDto
    {
        /// <summary>
        /// The unique identifier of the person.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// The person's identification number.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string IdNumber { get; set; } = string.Empty;

        /// <summary>
        /// The person's first name.
        /// </summary>
        [StringLength(50)]
        public string? Name { get; set; }

        /// <summary>
        /// The person's surname.
        /// </summary>
        [StringLength(50)]
        public string? Surname { get; set; }
    }
}
