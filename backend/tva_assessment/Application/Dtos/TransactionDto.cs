namespace tva_assessment.Application.DTOs
{
    /// <summary>
    /// Represents a transaction returned by the API.
    /// </summary>
    public class TransactionDto
    {
        /// <summary>
        /// The unique identifier of the transaction.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// The identifier of the related account.
        /// </summary>
        public int AccountCode { get; set; }

        /// <summary>
        /// The date the transaction occurred.
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// The date the transaction was captured.
        /// </summary>
        public DateTime CaptureDate { get; set; }

        /// <summary>
        /// The monetary amount of the transaction.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The description of the transaction.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
