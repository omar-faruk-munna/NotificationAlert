using System.Collections.Generic;
using System.Xml.Serialization;

namespace NotificationAlert.Api.Models
{
    [XmlRoot(ElementName = "REPLY")]
    public class SslSmsReply
    {
        [XmlElement(ElementName = "PARAMETER")]
        public string PARAMETER { get; set; }
        [XmlElement(ElementName = "LOGIN")]
        public string LOGIN { get; set; }
        [XmlElement(ElementName = "PUSHAPI")]
        public string PUSHAPI { get; set; }
        [XmlElement(ElementName = "STAKEHOLDERID")]
        public string STAKEHOLDERID { get; set; }
        [XmlElement(ElementName = "PERMITTED")]
        public string PERMITTED { get; set; }
        [XmlElement(ElementName = "SMSINFO")]
        public List<SslSmsInfo> SMSINFO { get; set; }
    }
}
