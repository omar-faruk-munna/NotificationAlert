using System.Text.Json.Serialization;

namespace NotificationAlertWinService.Models
{
    public class ServiceException
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("messageId")]
        public string MessageId { get; set; }
    }

    public class RequestError
    {
        [JsonPropertyName("serviceException")]
        public ServiceException ServiceException { get; set; }
    }

    public class InfobipErrorResponse
    {
        [JsonPropertyName("requestError")]
        public RequestError RequestError { get; set; }
    }
}
