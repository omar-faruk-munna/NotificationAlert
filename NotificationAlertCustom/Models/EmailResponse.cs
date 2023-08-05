using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationAlertCustom.Models
{
    public class EmailResponse
    {
        public int IsEmailSuccess { get; set; }
        public string EmailReasonForFail { get; set; }
        public string EmailSentTime { get; set; }
    }
}
