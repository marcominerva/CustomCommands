using DeviceControl.Models;
using DeviceControl.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DeviceControl.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
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
        public async Task<IActionResult> Execute(DeviceCommand command)
        {
            //await gpioService.SetPinAsync(14, PinValue.High);
            return NoContent();
        }
    }
}
