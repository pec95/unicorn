using System;
using System.ComponentModel.DataAnnotations;

namespace Unicorn.Models
{
    public class PartAdd
    {
        [Required]
        public int SerialNumber { get; set; }
        [Required]
        public string DateManufactured { get; set; }
        [Required]
        public int CarId { get; set; }
    }
}
