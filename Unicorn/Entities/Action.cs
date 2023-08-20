using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Unicorn.Entities
{
    public class Action
    {
        [Key]
        public int Id { get; set; }

        public DateTime ActionStart { get; set; }
        public DateTime ActionEnd { get; set; }
        public float DiscountPercent { get; set; }

        public virtual ICollection<Article> Articles { get; set; }
    }
}
