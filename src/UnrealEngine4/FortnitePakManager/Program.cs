using FModel.Methods;
using FModel.Methods.AESManager;
using FModel.Methods.Assets;
using FModel.Methods.PAKs;
using FModel.Methods.Utilities;
using FortnitePakManager;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            SpecialExport2 special = new SpecialExport2()
                .Init()
                .ExportAll();



            Console.ReadLine();
        }

    }
}
