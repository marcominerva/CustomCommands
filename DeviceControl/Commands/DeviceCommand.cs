using System.ComponentModel.DataAnnotations;

namespace DeviceControl.Commands
{
    public class DeviceCommand
    {
        [Required]
        public string DeviceName { get; set; }

        [Required]
        public string OnOff { get; set; }
    }
}
