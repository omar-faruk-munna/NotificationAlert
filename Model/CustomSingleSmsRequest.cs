using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class CustomSingleSmsRequest
    {
        [Required]
        public int MessageFunctionId { get; set; }
        [Required]
        public bool IsBanglaContent { get; set; }
        public string SmsOperator { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 11)]
        public string ToNumber { get; set; }
        [Required]
        public string SmsContent { get; set; }
    }
}
