namespace NotificationAlert.Api.Models
{
    public class DbConString
    {
        public string CONNECTION_ID { get; set; }
        public string CONNECTION_NM { get; set; }
        public string APPLICATION_ID { get; set; }
        public string CONN_DB_NM { get; set; }
        public string CONN_DB_TYPE { get; set; }
        public string CONN_SCHEMA_NM { get; set; }
        public string CONN_USER_ID { get; set; }
        public string CONN_PASS_WORD { get; set; }
        public string DEFAULT_CONN_FLAG { get; set; }
        public string MAKE_BY { get; set; }
        public string MAKE_DT { get; set; }
        public object CloneObj { get; set; }
        public bool isAdd { get; set; }
        public bool isDelete { get; set; }
        public bool isOld { get; set; }
        public object CreateByLoginId { get; set; }
        public object MakeByLoginId { get; set; }
        public object AuthByLoginId { get; set; }
    }
}
