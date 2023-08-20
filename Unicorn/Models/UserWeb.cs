using System.ComponentModel.DataAnnotations;

namespace Unicorn.Models
{
    public class UserWeb
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
