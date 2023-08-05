using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class SmsAlertRequest : RootRequest
    {
        [Required]
        public string IsMultiple { get; set; }
        [Required]
        public string ContentType { get; set; }
        [Required]
        public string SmsContent { get; set; }
        [Required]
        public string BranchId { get; set; }
        [Required]
        public string AccountNo { get; set; }
    }
}
