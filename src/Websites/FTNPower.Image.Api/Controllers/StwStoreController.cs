using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using Newtonsoft.Json;
using FTNPower.Image.Processing;
using FTNPower.Image.Processing.Models;
using FTNPower.Image.Api;

namespace FTNPower.Image.Api.Controllers
{
    [Route("api/StwStore")]
    public class StwStoreController : CommonController
    {
        readonly ImageProcessorQ imgProcq = null;
        public StwStoreController(ImageProcessorQ prsq)
        {
            imgProcq = prsq;
        }
        //https://docs.microsoft.com/tr-tr/dotnet/api/system.windows.media.radialgradientbrush?view=netframework-4.8
        [HttpPost]
        [Produces("image/png")]
        [ResponseCache(NoStore = true)]
        [Obsolete]
        public IActionResult Post([FromBody] StwStoreModel stwStoreModel)
        {
            try
            {
                if (!IsAuthorized())
                    return Unauthorized();

                if (!ModelState.IsValid)
                {
                    IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                    return Content(JsonConvert.SerializeObject(allErrors));
                    // return StatusCode(406);//not acceptable
                }

                if (stwStoreModel == null)
                {
                    return NotFound();
                }

                var stream = imgProcq.Draw_StwStore2(stwStoreModel);
                return File(stream, "image/png");
            }
            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e));
            }
        }
    }
}