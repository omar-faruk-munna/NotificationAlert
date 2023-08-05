using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class SmsRequest : RootRequest
    {
        public string Operator { get; set; }
        [Required]
        public string ToNumber { get; set; }
        [Required]
        public string ContentType { get; set; }
        [Required]
        public string SmsContent { get; set; }
    }
}
