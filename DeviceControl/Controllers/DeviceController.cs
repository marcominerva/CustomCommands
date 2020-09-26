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
        private readonly LedService ledService;

        public DeviceController(LedService ledService)
        {
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
        [HttpPost("led")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> LedCommand(LedCommand command)
        {
            await ledService.SetColorAsync(command);
            return NoContent();
        }
    }
}
