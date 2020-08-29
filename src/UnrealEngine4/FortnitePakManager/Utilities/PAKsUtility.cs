using PakReader;
using System;
using System.IO;
using System.Linq;
using System.Text;
using FProp = FortnitePakManager.Settings;

namespace FModel.Methods.Utilities
{
    static class PAKsUtility
    {
        public static string GetPAKGuid(string PAKPath)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(PAKPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                reader.BaseStream.Seek(-FPakInfo.Size, SeekOrigin.End);
                FGuid gd = new FGuid(reader);
                return gd.ToString();
            }
        }

        public static string GetEpicGuid(string PAKGuid)
        {
            StringBuilder sB = new StringBuilder();
            foreach (string part in PAKGuid.Split('-'))
            {
                sB.Append(Int64.Parse(part).ToString("X8"));
            }
            return sB.ToString();
        }

        public static bool IsPAKLocked(FileInfo PakFileInfo)
        {
            FileStream stream = null;
            try
            {
                stream = PakFileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null) { stream.Close(); }
            }

            return false;
        }


    }
}
