using Newtonsoft.Json;

namespace Model
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
}
