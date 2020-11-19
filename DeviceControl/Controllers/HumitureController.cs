using DeviceControl.Models;
using DeviceControl.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DeviceControl.Controllers
{
    public class HumitureController : ControllerBase
    {
        private readonly HumitureService humitureService;

        public HumitureController(HumitureService humitureService)
        {
            this.humitureService = humitureService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Humiture), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Get()
        {
            var humiture = await humitureService.GetHumitureAsync();
            if (humiture == null)
            {
                return StatusCode(StatusCodes.Status504GatewayTimeout);
            }

            return Ok(humiture);
        }
    }
}
