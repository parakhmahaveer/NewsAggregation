namespace NewsAggrigation.API.ServiceDTOs.RequestDTOs
{
    public class SearchRequest
    {
        public string Query { get; set; }
        public string StartDate { get; set; } // Format: yyyy-MM-dd
        public string EndDate { get; set; }
        public string SortBy { get; set; }
    }
}
