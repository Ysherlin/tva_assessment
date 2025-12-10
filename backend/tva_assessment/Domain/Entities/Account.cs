namespace tva_assessment.Domain.Entities
{
    /// <summary>
    /// Represents a financial account belonging to a person.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// The unique identifier of the account.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// The identifier of the person who owns the account.
        /// </summary>
        public int PersonCode { get; set; }

        /// <summary>
        /// The unique number of the account.
        /// </summary>
        public required string AccountNumber { get; set; }

        /// <summary>
        /// The current balance of the account.
        /// </summary>
        public decimal OutstandingBalance { get; set; }

        /// <summary>
        /// The person who owns the account.
        /// </summary>
        public Person? Person { get; set; }

        /// <summary>
        /// The status of the account.
        /// </summary>
        public AccountStatus? Status { get; set; }

        /// <summary>
        /// The transactions linked to the account.
        /// </summary>
        public List<Transaction> Transactions { get; set; } = new();
    }
}
