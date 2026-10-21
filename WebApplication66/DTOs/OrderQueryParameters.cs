namespace WebApplication66.DTOs
{
    public class OrderQueryParameters
    {
        public string? Number { get; set; }
        public decimal? MinTotal { get; set; }
        public decimal? MaxTotal { get; set; }
        public string? SortBy { get; set; } = "date_desc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
