
using FTNPower.Image.Api;
using FTNPower.Image.Api.Service;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FTNPower.Image.Api.Controllers
{
    [Route("f")]
    public class FileController : CommonController
    {

        //f/FTNImages.json
        [HttpGet]
        [Produces("application/json")]
        [Route("FTNImages.json")]
        [ResponseCache(NoStore = true)]
        public IActionResult Get()
        {
            try
            {
                if (!IsAuthorized())
                    return Unauthorized();

                var path = $"_CDNIMGIDS.json".GetPhysFile("FTNPower");
                if (path.StartsWith("@@"))
                    return NotFound();

                return PhysicalFile(path, "application/json");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}