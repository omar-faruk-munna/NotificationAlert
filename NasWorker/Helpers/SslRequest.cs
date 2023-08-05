using Newtonsoft.Json;

namespace NasWorker.Helpers
{
    public class SslRequest
    {
        [JsonProperty("api_token")]
        public string ApiToken { get; set; }

        [JsonProperty("sid")]
        public string Sid { get; set; }

        [JsonProperty("msisdn")]
        public string Msisdn { get; set; }

        [JsonProperty("sms")]
        public string Sms { get; set; }

        [JsonProperty("csms_id")]
        public string CsmsId { get; set; }
    }
}
