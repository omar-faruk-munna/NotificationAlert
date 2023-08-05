using Newtonsoft.Json;
using System.Collections.Generic;

namespace WorkerServiceCustom.Helpers
{
    public class Destination
    {
        [JsonProperty("to")]
        public string To { get; set; }
    }

    public class SmsMessage
    {
        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("destinations")]
        public List<Destination> Destinations { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class InfobipSmsRequest
    {
        [JsonProperty("messages")]
        public List<SmsMessage> Messages { get; set; }
    }
}
