using Newtonsoft.Json;
using System.Collections.Generic;

namespace NasWorker.Helpers
{
    public class SslResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("status_code")]
        public int StatusCode { get; set; }

        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }

        [JsonProperty("smsinfo")]
        public List<Smsinfo> Smsinfo { get; set; }
    }

    public class Smsinfo
    {
        [JsonProperty("sms_status")]
        public string SmsStatus { get; set; }

        [JsonProperty("status_message")]
        public string StatusMessage { get; set; }

        [JsonProperty("msisdn")]
        public string Msisdn { get; set; }

        [JsonProperty("sms_type")]
        public string SmsType { get; set; }

        [JsonProperty("sms_body")]
        public string SmsBody { get; set; }

        [JsonProperty("csms_id")]
        public string CsmsId { get; set; }

        [JsonProperty("reference_id")]
        public string ReferenceId { get; set; }
    }
}
