namespace NewsAggrigationClient.Models.DTOs.ResponseDTOs
{
    public class NotificationConfigResponse
    {
        public List<CategoryStatusDto> Categories { get; set; }
        public List<string> Keywords { get; set; }
    }
}
