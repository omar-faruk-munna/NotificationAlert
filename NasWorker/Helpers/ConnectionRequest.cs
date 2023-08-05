namespace NasWorker.Helpers
{
    public class ConnectionRequest
    {
        public object RequestId { get; set; }
        public string RequestCliedIP { get; set; }
        public string RequestCliedAgent { get; set; }
        public string RequestAppIP { get; set; }
        public string RequestAppBaseUrl { get; set; }
        public string BusinessData { get; set; }
        public string FunctionId { get; set; }
        public string BranchId { get; set; }
        public string UserId { get; set; }
        public object InstitueId { get; set; }
        public string SessionId { get; set; }
        public string RequestDateTime { get; set; }
        public int SessionTimeout { get; set; }
    }
}
