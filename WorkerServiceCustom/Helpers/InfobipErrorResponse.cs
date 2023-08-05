using Newtonsoft.Json;

namespace WorkerServiceCustom.Helpers
{
    public class ServiceException
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("messageId")]
        public string MessageId { get; set; }
    }

    public class RequestError
    {
        [JsonProperty("serviceException")]
        public ServiceException ServiceException { get; set; }
    }

    public class InfobipErrorResponse
    {
        [JsonProperty("requestError")]
        public RequestError RequestError { get; set; }
    }
}
