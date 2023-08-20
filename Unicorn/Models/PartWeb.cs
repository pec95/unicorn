using System.ComponentModel.DataAnnotations;

namespace Unicorn.Models
{
    public class PartWeb
    {
        public int Id { get; set; }
        public int SerialNumber { get; set; }
        public string DateManufactured { get; set; }
    }
}
