using FTNPower.Image.Api.Service.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace FTNPower.Image.Api.Service
{
    public static partial class Extensions
    {
        public static bool IsHero(this AssetDat data)
        {
            return data.ClassType == "Hero";
        }
        public static bool IsWeapon(this AssetDat data)
        {
            return data.ClassType == "Weapon";
        }
        public static bool IsDefender(this AssetDat data)
        {
            return data.ClassType == "Defender";
        }
        public static int GetRarity(this AssetDat data)
        {
            switch (data.Rarity)
            {
                case "Mythic":
                    return 6;

                case "Legendary":
                    return 5;

                case "Epic":
                    return 4;

                case "Rare":
                    return 3;

                case "UnCommon":
                    return 2;

                case "Common":
                    return 1;

                case null:
                    return 0;

                default:
                    return -1;
            }
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }


        public static bool IsDirectoryExists(this string dir)
        {
            return Directory.Exists(dir);
        }

        public static string StoragePath { get; internal set; }

        public static void SetStoragePath()
        {
            try
            {
                StoragePath = Path.Combine(AppContext.BaseDirectory, "Storage");
#if DEBUG
                StoragePath = StoragePath.Replace("\\FTNPower.Image.Api.Tests", "\\FTNPower.Image.Api");//Test-Image files retriewing from api project's folder
#endif
                if (!StoragePath.IsDirectoryExists())
                    Directory.CreateDirectory(StoragePath);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string GetPhysFile(this string fileName, params string[] paths)
        {
            List<string> pths = new List<string>();
            pths.Add(StoragePath);
            if (paths != null)
                pths.AddRange(paths);
            pths.Add(fileName);
            var location = Path.GetFullPath(Path.Combine(pths.ToArray()));
            if (!File.Exists(location))
                return $"@@{location}";

            return location;
        }
    }
}