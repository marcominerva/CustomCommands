using System.ComponentModel.DataAnnotations;

namespace DeviceControl.Commands
{
    public class LedCommand
    {
        [Required]
        public string Color { get; set; }
    }
}
