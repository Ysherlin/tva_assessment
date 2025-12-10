namespace tva_assessment.Domain.Entities
{
    /// <summary>
    /// Represents the status of an account.
    /// </summary>
    public class AccountStatus
    {
        /// <summary>
        /// The identifier of the related account.
        /// </summary>
        public int AccountCode { get; set; }

        /// <summary>
        /// A value indicating whether the account is closed.
        /// </summary>
        public bool IsClosed { get; set; }

        /// <summary>
        /// The account that owns this status.
        /// </summary>
        public Account Account { get; set; } = null!;
    }
}
