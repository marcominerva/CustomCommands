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
        private readonly LedService ledService;

        public DeviceController(GpioService gpioService, LedService ledService)
        {
            this.gpioService = gpioService;
            this.ledService = ledService;
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

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPost("led")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> LedCommand(LedCommand command)
        {
            await ledService.SetColorAsync(command);
            return NoContent();
        }
    }
}
