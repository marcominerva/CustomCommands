using System.ComponentModel.DataAnnotations;

namespace DeviceControl.Models
{
    public class DeviceCommand
    {
        [Required]
        public string DeviceName { get; set; }

        [Required]
        public string OnOff { get; set; }
    }
}
