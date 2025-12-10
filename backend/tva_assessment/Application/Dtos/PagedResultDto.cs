namespace tva_assessment.Application.DTOs
{
    /// <summary>
    /// Represents a page of items.
    /// </summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    public class PagedResultDto<T>
    {
        /// <summary>
        /// The items in the current page.
        /// </summary>
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();

        /// <summary>
        /// The current page number.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The size of the page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of items.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// The total number of pages.
        /// </summary>
        public int TotalPages { get; set; }
    }
}
