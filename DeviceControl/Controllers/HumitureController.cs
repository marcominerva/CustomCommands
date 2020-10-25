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

        [ProducesResponseType(typeof(Humiture), StatusCodes.Status200OK)]
        [HttpGet("")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Get()
        {
            var humiture = await humitureService.GetHumitureAsync();
            return Ok(humiture);
        }
    }
}
