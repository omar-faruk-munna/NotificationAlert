using System.Collections.Generic;

namespace NotificationAlert.Api.Models
{
    public class SmsModelWrap
    {
        public string Message { get; set; }
        public List<SmsModel> ListSmsModel { get; set; }
    }

    public class SmsModel
    {
        public string Operator { get; set; }
        public string Number { get; set; }
        public string AccountNo { get; set; }
    }
}
