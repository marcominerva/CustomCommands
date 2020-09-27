using System.ComponentModel.DataAnnotations;

namespace DeviceControl.Models
{
    public class LedCommand
    {
        [Required]
        public string Color { get; set; }
    }
}
