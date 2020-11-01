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

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPost("")]
        [ProducesDefaultResponseType]
        public IActionResult DeviceCommand(DeviceCommand command)
        {
            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPost("pin")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PinCommand(PinCommand command)
        {
            await gpioService.SetPinAsync(command);
            return NoContent();
        }
    }
}
