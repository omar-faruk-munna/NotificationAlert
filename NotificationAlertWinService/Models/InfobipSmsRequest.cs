using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NotificationAlertWinService.Models
{
    public class Destination
    {
        [JsonPropertyName("to")]
        public string To { get; set; }
    }

    public class SmsMessage
    {
        [JsonPropertyName("from")]
        public string From { get; set; }

        [JsonPropertyName("destinations")]
        public List<Destination> Destinations { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class InfobipSmsRequest
    {
        [JsonPropertyName("messages")]
        public List<SmsMessage> Messages { get; set; }
    }
}
