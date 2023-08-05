using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class RootRequest
    {
        [Required]
        public string MessageFunctionId { get; set; }
    }
}
