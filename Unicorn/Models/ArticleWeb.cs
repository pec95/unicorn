using System.ComponentModel.DataAnnotations;

namespace Unicorn.Models
{
    public class ArticleWeb
    {
        public int Id { get; set; }
        [Required]
        public int SerialNumber { get; set; }
        public string DateManufactured { get; set; }
        [Required]
        public float Price { get; set; }
        public bool Action { get; set; }

    }
}
