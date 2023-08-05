using System.Collections.Generic;
using Newtonsoft.Json;

namespace NotificationAlertCustom.Models
{
    public class Status
    {
        [JsonProperty("groupId")]
        public int GroupId { get; set; }

        [JsonProperty("groupName")]
        public string GroupName { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class Message
    {
        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }

        [JsonProperty("messageId")]
        public string MessageId { get; set; }
    }

    public class InfobipSuccessResponse
    {
        [JsonProperty("bulkId")]
        public string BulkId { get; set; }

        [JsonProperty("messages")]
        public List<Message> Messages { get; set; }
    }
}
