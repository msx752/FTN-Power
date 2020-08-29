using Microsoft.AspNetCore.Mvc;

namespace FTNPower.Image.Api.Controllers
{
    [Route("Storage/FTNPower")]
    public class BlockedController : ControllerBase
    {
        [HttpGet]
        [Route("im")]
        public IActionResult blockFolder()
        {
            return NotFound();
        }

        [HttpGet]
        [Route("im/{id}")]
        public IActionResult blockImages([FromRoute]string id)
        {
            return NotFound();
        }

        [HttpGet]
        [Route("im/_CDNIMGIDS.json")]
        public IActionResult blockJson()
        {
            return NotFound();
        }
    }
}