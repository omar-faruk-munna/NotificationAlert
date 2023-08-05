namespace GdicNasWorker.Entities
{
    public class Customer
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public int sms_flag { get; set; }
        public int email_flag { get; set; }
        public int alert_flag { get; set; }
        public string alert_resp { get; set; }
    }
}
