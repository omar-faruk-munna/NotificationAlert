using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NotificationAlertWinService.Models
{
    public class Status
    {
        [JsonPropertyName("groupId")]
        public int GroupId { get; set; }

        [JsonPropertyName("groupName")]
        public string GroupName { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("to")]
        public string To { get; set; }

        [JsonPropertyName("status")]
        public Status Status { get; set; }

        [JsonPropertyName("messageId")]
        public string MessageId { get; set; }
    }

    public class InfobipSuccessResponse
    {
        [JsonPropertyName("bulkId")]
        public string BulkId { get; set; }

        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; }
    }
}
