using DeviceControl.Commands;
using DeviceControl.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DeviceControl.Controllers
{
    public class LedController : ControllerBase
    {
        private readonly LedService ledService;

        public LedController(LedService ledService)
        {
            this.ledService = ledService;
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPost("")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> SetLed(LedCommand command)
        {
            await ledService.SetColorAsync(command);
            return NoContent();
        }
    }
}
