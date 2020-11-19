using DeviceControl.Commands;
using DeviceControl.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DeviceControl.Controllers
{
    public class DeviceController : ControllerBase
    {
        private readonly GpioService gpioService;

        public DeviceController(GpioService gpioService)
        {
            this.gpioService = gpioService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public IActionResult DeviceCommand(DeviceCommand command)
        {
            return NoContent();
        }

        [HttpPost("pin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PinCommand(PinCommand command)
        {
            await gpioService.SetPinAsync(command);
            return NoContent();
        }
    }
}
