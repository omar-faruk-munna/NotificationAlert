using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationAlertCustom.Models
{
    public class AlertRequest
    {
        public int IsMultiple { get; set; }
        public int IsSms { get; set; }
        public string SmsContent { get; set; }
        public int IsEmail { get; set; }
        public int IsHtmlBody { get; set; }
        public string EmailSubject { get; set; }
        public string EmailContent { get; set; }
        public string BranchId { get; set; }
        public string AccountNo { get; set; }
    }
}
