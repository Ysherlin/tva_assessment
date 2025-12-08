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
        public string IdNumber { get; set; } = string.Empty;

        /// <summary>
        /// The person's first name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The person's surname.
        /// </summary>
        public string? Surname { get; set; }
    }
}
