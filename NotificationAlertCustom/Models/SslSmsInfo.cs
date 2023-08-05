using System.Xml.Serialization;

namespace NotificationAlertCustom.Models
{
    [XmlRoot(ElementName = "SMSINFO")]
    public class SslSmsInfo
    {
        [XmlElement(ElementName = "MSISDN")]
        public string MSISDN { get; set; }
        [XmlElement(ElementName = "SMSTEXT")]
        public string SMSTEXT { get; set; }
        [XmlElement(ElementName = "CSMSID")]
        public string CSMSID { get; set; }
        [XmlElement(ElementName = "REFERENCEID")]
        public string REFERENCEID { get; set; }
    }
}
