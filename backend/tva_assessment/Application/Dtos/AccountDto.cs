namespace tva_assessment.Application.DTOs
{
    /// <summary>
    /// Represents an account returned by the API.
    /// </summary>
    public class AccountDto
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
        public string AccountNumber { get; set; } = string.Empty;

        /// <summary>
        /// The current balance of the account.
        /// </summary>
        public decimal OutstandingBalance { get; set; }
    }
}
