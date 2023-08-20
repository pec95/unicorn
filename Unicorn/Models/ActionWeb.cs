using System.ComponentModel.DataAnnotations;

namespace Unicorn.Models
{
    public class ActionWeb
    {
        public int Id { get; set; }
        [Required]
        public string StartDate { get; set; }
        [Required]
        public string EndDate { get; set; }
        [Required]
        public int[] Articles { get; set; }
        [Required]
        public float Discount { get; set; }
    }
}
