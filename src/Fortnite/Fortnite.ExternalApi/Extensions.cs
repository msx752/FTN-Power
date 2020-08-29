using Fortnite.Api;
using Fortnite.Core.Services;
using fortniteLib.Responses.Catalog;
using Global;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fortnite.External
{
    public static class Extensions
    {
        public async static Task<string> SaveStwStoreToLocalAsync(this IEpicApi ieapi, bool forceDownload = true)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "savedStwStore.png");
            FileInfo fif = new FileInfo(filePath);
            if (!forceDownload)
            {
                var elapsedHours = (DateTime.UtcNow - fif.LastWriteTime.ToUniversalTime()).TotalHours;
                if (fif.Exists && elapsedHours <= 23)
                {
                    return filePath;
                }
            }

            if (fif.Exists)
                fif.Delete();

            using (HttpClient client = new HttpClient())
            {
                var queueApi = DIManager.Services.GetRequiredService<IFortniteQueueApi>();
                var stwStore = queueApi.STWStoreSimplified();
                var strjson = JsonConvert.SerializeObject(stwStore);
                var response = await client.PostAsync($"https://cdn.ftnpower.com/api/StwStore?aKey={DIManager.Services.ImageServiceConfigs().AKey}", new StringContent(strjson, Encoding.UTF8, "application/json"));
                using (var b = Image.FromStream(await response.Content.ReadAsStreamAsync()))
                {
                    ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders().First(f => f.MimeType == "image/png");
                    EncoderParameters eParams = new EncoderParameters(1);
                    eParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                    b.Save(filePath, codec, eParams);
                }
            }
            return filePath;
        }
        public static string SetStwStoreTitle(this DateTimeOffset utcNow)
        {
            var title = $"**S.T.W. Weekly Store** [ *{utcNow.ToString("dd MMMM yyyy")}* ]";
            return title;
        }
    }
}