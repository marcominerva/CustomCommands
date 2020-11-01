using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace DeviceControl.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
    {
    }
}
