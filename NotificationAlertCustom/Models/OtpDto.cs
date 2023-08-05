using System.Collections.Generic;

namespace NotificationAlertCustom.Models
{
    public class OtpRequest
    {
        public int IsSms { get; set; }
        public string SmsContent { get; set; }
        public string Operator { get; set; }
        public string ToNumber { get; set; }
        public int IsEmail { get; set; }
        public int IsHtmlBody { get; set; }
        public string EmailBody { get; set; }
        public string ToEmail { get; set; }
        public string EmailSubject { get; set; }
    }

    public class OtpResponse
    {
        public OtpRes Result { get; set; }
    }

    public class OtpListResponse
    {
        public List<OtpRes> Result { get; set; }
    }

    public class OtpRes
    {
        public int IsSmsSuccess { get; set; }
        public string SmsReasonForFail { get; set; }
        public int IsEmailSuccess { get; set; }
        public string EmailReasonForFail { get; set; }
        public string SentTime { get; set; }
    }
}
