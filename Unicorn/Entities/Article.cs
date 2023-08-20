using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Unicorn.Entities
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Part")]
        public int PartId { get; set; }
        public Part Part { get; set; }

        public int Serial { 
            get { 
                return Part.SerialNumber;
            }
        }

        public DateTime DateManufactured
        {
            get
            {
                return Part.ManufacterDate;
            }
        }

        [Required]
        public float BasePrice { get; set; }

        [ForeignKey("Action")]
        public int? ActionId { get; set; }
        public Action Action { get; set; }
    }
}
