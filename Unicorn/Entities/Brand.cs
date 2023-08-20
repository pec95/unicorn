using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Unicorn.Entities
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Car> Cars { get; set; }
    }
}
