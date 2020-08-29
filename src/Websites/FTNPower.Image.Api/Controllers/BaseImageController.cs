using FTNPower.Image.Processing;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace FTNPower.Image.Api.Controllers
{
    public class BaseImageController : ControllerBase
    {
        internal IActionResult GetImage(Bitmap b)
        {
            ushort w = 0;
            if (Request.Query.ContainsKey("w"))
            {
                if (!ushort.TryParse(Request.Query["w"], out w))
                    return StatusCode(406);
            }
            return File(b.GetStream(w), "image/png");
        }

        internal IActionResult GetImage(byte[] rarity)
        {
            ushort w = 0;
            if (Request.Query.ContainsKey("w"))
            {
                if (!ushort.TryParse(Request.Query["w"], out w))
                    return StatusCode(406);
            }
            return File(rarity.GetImage(w), "image/png");
        }
    }
}