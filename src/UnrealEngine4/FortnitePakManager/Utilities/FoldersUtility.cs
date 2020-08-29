using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using FProp = FortnitePakManager.Settings;

namespace FModel.Methods.Utilities
{
    static class FoldersUtility
    {
        /// <summary>
        /// FOutput_Path should never be empty so no need to check
        /// </summary>
        public static void OpenOutputFolder()
        {
            Process.Start(@"" + FProp.FOutput_Path);
        }

        /// <summary>
        /// open file with the default program
        /// </summary>
        /// <param name="path"></param>
        public static void OpenWithDefaultProgram(string path)
        {
            if (Directory.Exists(Path.GetDirectoryName(path)))
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        Arguments = "\"" + path + "\"",
                        FileName = "explorer.exe"
                    };
                    Process.Start(startInfo);
                }
                catch (Exception)
                {
                  //  new UpdateMyConsole("Error while trying to open ", CColors.White).Append();
                   // new UpdateMyConsole(path, CColors.Red, true).Append();
                }
            }
            else
            {
              //  new UpdateMyConsole(path, CColors.Blue).Append();
               // new UpdateMyConsole(" Directory does not exist!", CColors.White, true).Append();
            }
        }

        /// <summary>
        /// at startup
        /// </summary>
        public static void LoadFolders()
        {
            FixNoOutputPath();
            CreateDefaultSubFolders();
        }

        /// <summary>
        /// if FOutput_Path is empty the .Exe directory will be FOutput_Path
        /// </summary>
        private static void FixNoOutputPath()
        {
            if (string.IsNullOrEmpty(FProp.FOutput_Path))
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "Output";
                FProp.FOutput_Path = path;
             //   FProp.Default.Save();

                DebugHelper.WriteLine("No output path, auto set to " + path);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void CreateDefaultSubFolders()
        {
            //3.0-
            if (File.Exists(FProp.FOutput_Path + "\\AESManager.xml")) { File.Delete(FProp.FOutput_Path + "\\AESManager.xml"); }
          //  if (Directory.Exists(FProp.FOutput_Path + "\\Backup\\")) { Directory.Delete(FProp.FOutput_Path + "\\Backup\\", true); }
         //   if (Directory.Exists(FProp.FOutput_Path + "\\Extracted\\")) { Directory.Delete(FProp.FOutput_Path + "\\Extracted\\", true); }
         //   if (Directory.Exists(FProp.FOutput_Path + "\\Saved_JSON\\")) { Directory.Delete(FProp.FOutput_Path + "\\Saved_JSON\\", true); }

            Directory.CreateDirectory(FProp.FOutput_Path + "\\FTNPower\\");
            Directory.CreateDirectory(FProp.FOutput_Path + "\\FTNPower\\im\\");
        //    Directory.CreateDirectory(FProp.FOutput_Path + "\\Backups\\");
        //    Directory.CreateDirectory(FProp.FOutput_Path + "\\Exports\\");
         //   Directory.CreateDirectory(FProp.FOutput_Path + "\\Icons\\");
        //    Directory.CreateDirectory(FProp.FOutput_Path + "\\JSONs\\");
       //     Directory.CreateDirectory(FProp.FOutput_Path + "\\Sounds\\");
         //   Directory.CreateDirectory(FProp.FOutput_Path + "\\Logs\\");

            DebugHelper.WriteLine("Folders created if they didn't exist");
        }

        public static string GetFullPathWithoutExtension(string path)
        {
            return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)).Replace("\\", "/");
        }

        public static string FixFortnitePath(string path)
        {
            Regex regexGame = new Regex(Regex.Escape("Game"));
            string fixedPath = regexGame.Replace(path, "FortniteGame/Content", 1);
            int sep = fixedPath.LastIndexOf('.');
            return fixedPath.Substring(0, sep > 0 ? sep : fixedPath.Length);
        }

       

    }
}
