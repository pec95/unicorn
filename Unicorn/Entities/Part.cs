using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace Unicorn.Entities
{
    public class Part
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SerialNumber { get; set; }
        [Required]
        public DateTime ManufacterDate { get; set; }

        public virtual ICollection<Car> Cars { get; set; }
    }
}
