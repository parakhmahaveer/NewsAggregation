using NewsAggrigation.API.DataDTOs;

namespace NewsAggrigation.API.ServiceDTOs.ResponseDTOs
{
    public class NotificationConfigResponse
    {
        public List<CategoryStatusDto> Categories { get; set; }
        public List<string> Keywords { get; set; }
    }
}
