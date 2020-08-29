
using FTNPower.Image.Api.Service;
using FTNPower.Image.Processing.Properties;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace FTNPower.Image.Api.Controllers
{
    [Route("i")]
    public class ImageController : BaseImageController
    {
        //i/cb23c27c14434ac180348e56f03d5d65.png
        [HttpGet]
        [Route("{hashedId:regex(^[[a-z0-9]]{{32,33}}$)}.png")]
        [ResponseCache(Duration = 604800, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult Get([FromRoute]string hashedId)
        {
            var id = ImageProvider.ISVC.GetId(hashedId);
            if (id.IsNullOrWhiteSpace())
                return NotFound();

            var path = $"{id}.png".GetPhysFile("FTNPower", "im");
            if (path.StartsWith("@@"))
                return NotFound();

            return GetImage(System.IO.File.ReadAllBytes(path));
        }

        [HttpGet]
        [Route("Mythic.png")]
        [ResponseCache(Duration = 604800, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult Mythic()
        {
            return GetImage(Resources.Mythic);
        }

        [HttpGet]
        [Route("Legendary.png")]
        [ResponseCache(Duration = 604800, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult Legendary()
        {
            return GetImage(Resources.Legendary);
        }

        [HttpGet]
        [Route("Epic.png")]
        [ResponseCache(Duration = 604800, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult Epic()
        {
            return GetImage(Resources.Epic);
        }

        [HttpGet]
        [Route("Rare.png")]
        [ResponseCache(Duration = 604800, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult Rare()
        {
            return GetImage(Resources.Rare);
        }

        [HttpGet]
        [Route("UnCommon.png")]
        [ResponseCache(Duration = 604800, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult UnCommon()
        {
            return GetImage(Resources.UnCommon);
        }

        [HttpGet]
        [Route("Common.png")]
        [ResponseCache(Duration = 604800, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult Common()
        {
            return GetImage(Resources.Common);
        }

    }
}