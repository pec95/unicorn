using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Unicorn.Entities
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Brand")]
        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Part> Parts { get; set; }

    }
}
