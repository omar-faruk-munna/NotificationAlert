using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class MultipleSmsRequest : RootRequest
    {
        [Required]
        public string ContentType { get; set; }
        [Required]
        public string SmsContent { get; set; }
        public List<Sms> SmsList { get; set; }
    }

    public class Sms
    {
        public string Operator { get; set; }
        [Required]
        public string ToNumber { get; set; }
    }
}
