namespace Asp.NETCoreApi.Dto {
    public class PaginatedDto<T> {
        public int TotalItems { get; set; } // Total number of items
        public int PageNumber { get; set; } // Current page number
        public int PageSize { get; set; } // Number of items per page
        public List<T> Items { get; set; } // List of items for the current page
    }
}
