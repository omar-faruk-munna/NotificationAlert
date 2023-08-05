using Newtonsoft.Json;

namespace WorkerServiceCustom.Helpers
{
    public class PushNotificationRequest
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("msg")]
        public string SmsContent { get; set; }

        [JsonProperty("toUserCustomerId")]
        public string CustomerId { get; set; }

        [JsonProperty("byUsername")]
        public string Username { get; set; }

        [JsonProperty("fromIp")]
        public string FromIp { get; set; }
    }

    public class PushNotificationResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string Result { get; set; }
    }
}
