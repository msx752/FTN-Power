using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FortnitePakManager
{
  public static  class Settings
    {
        public static string FPak_Path { get; set; } = "";
        public static string FOutput_Path { get; set; } = $"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output")}";
        public static string FPak_MainAES { get; set; } = "40AA2ED6FC28C429CA9E7795BDC6BC2A31E1B747571D4AE4B598943690CBA264";
        public static string FRarity_Design { get; set; } = "Default";
        public static string FLanguage { get; set; } = "English";
        public static bool FIsFeatured { get; set; } = true;
        public static bool FDiffFileSize { get; set; } = false;
        public static bool HeaderVisibility { get; set; } = true;
        public static bool ReloadAES { get; set; } = true;
        public static string ELauncherToken { get; set; } = "";
        public static long ELauncherExpiration { get; set; }
    }
}
