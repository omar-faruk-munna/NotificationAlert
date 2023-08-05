namespace Model
{
    public class EmailResponse
    {
        public int IsSuccess { get; set; }
        public string ReasonForFail { get; set; }
        public string EmailResponseTime { get; set; }
        public string EmailProiderId { get; set; }
        public string EmailProiderName { get; set; }
    }
}
