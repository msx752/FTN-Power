using FTNPower.Image.Api.Service.Models;
using Global;
using LinqKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace FTNPower.Image.Api.Service
{
    public class ImageService
    {
        public bool LocalProvider { get; private set; }
        public Uri ProviderUrl { get; private set; }
        public string ApplicationDir { get => Directory.GetParent(Extensions.StoragePath).FullName; }
        public string AKey { get; }

        public ImageService()
        {
            AKey = DIManager.Services.ImageServiceConfigs().AKey;
        }

        private IEnumerable<AssetDat> assetData { get; set; } = new List<AssetDat>();

        public string GetHash(string Id)
        {
            return assetData.Select(x => new { x.Id, x.HashedId })
                .FirstOrDefault(f => f.Id.Equals(Id))?.HashedId;
        }

        public string GetId(string hashedId)
        {
            return assetData.Select(x => new { x.Id, x.HashedId })
                .FirstOrDefault(f => f.HashedId.Equals(hashedId))?.Id;
        }

        [Obsolete]
        public IEnumerable<AssetDat> GetImageDatasByIds(params string[] keywords)
        {
            var predicate = PredicateBuilder.False<AssetDat>();

            foreach (string keyword in keywords)
            {
                string temp = keyword;

                predicate = predicate.Or(p => p.Id.EndsWith(temp, StringComparison.InvariantCultureIgnoreCase));
            }
            return assetData.AsQueryable().Where(predicate);
        }
        public string GetImagePhysLocation(string id)
        {
            var path = $"{id}.png".GetPhysFile("FTNPower", "im");
            if (path.StartsWith("@@"))
                return null;
            return path;
        }
        public ImageService Init(Uri loadFromRemoteUrl = null)
        {
            Extensions.SetStoragePath();
            if (loadFromRemoteUrl == null)
            {
                LocalProvider = true;
                var dir0 = "_CDNIMGIDS.json".GetPhysFile("FTNPower");
                if (dir0.StartsWith("@@"))
                {
                    throw new Exception($"UNDEFINED '_CDNIMGIDS.json' file location, loc:'{dir0}'");
                }
                else
                {
                    FileInfo fi0 = new FileInfo(dir0);
                    using (StreamReader sr0 = new StreamReader(fi0.FullName))
                    {
                        var jsn0 = sr0.ReadToEnd();
                        assetData = JsonConvert.DeserializeObject<List<AssetDat>>(jsn0).Distinct();
                    }
                }
            }
            else
            {
                LocalProvider = false;
                ProviderUrl = new Uri($"{loadFromRemoteUrl.Scheme}://{loadFromRemoteUrl.Host}:{loadFromRemoteUrl.Port}");
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        client.UseDefaultCredentials = true;
                        client.Encoding = Encoding.UTF8;
                        client.Headers.Add("Content-Type", "application/json");
                        var jsn0 = client.DownloadString(loadFromRemoteUrl);
                        assetData = JsonConvert.DeserializeObject<List<AssetDat>>(jsn0).Distinct();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            return this;
        }
    }
}