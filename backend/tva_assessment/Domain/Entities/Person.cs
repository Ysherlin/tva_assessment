namespace tva_assessment.Domain.Entities
{
    /// <summary>
    /// Represents an individual person in the system.
    /// </summary>
    public class Person
    {
        /// <summary>
        /// The unique identifier of the person.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// The person's identification number.
        /// </summary>
        public required string IdNumber { get; set; }

        /// <summary>
        /// The person's first name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The person's surname.
        /// </summary>
        public string? Surname { get; set; }

        /// <summary>
        /// The accounts that belong to the person.
        /// </summary>
        public List<Account> Accounts { get; set; } = new();
    }
}
