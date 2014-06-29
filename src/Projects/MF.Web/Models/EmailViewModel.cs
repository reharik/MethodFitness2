using System.ComponentModel.DataAnnotations;
using CC.Core.CustomAttributes;

namespace MF.Web.Models
{
    public class EmailViewModel
    {
        [Required]
        public string From { get; set; }
        [Required]
        public string To { get; set; }
        [Required]
        public string Subject { get; set; }
        [TextArea]
        [Required]
        public string Body { get; set; } 
    }
}