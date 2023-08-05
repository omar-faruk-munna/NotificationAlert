using System.Collections.Generic;

namespace Model
{
    public class RootResponse<T>
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<T> Results { get; set; }
    }
}
