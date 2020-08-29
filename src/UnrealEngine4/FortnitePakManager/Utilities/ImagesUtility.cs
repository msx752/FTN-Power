using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using FProp = FortnitePakManager.Settings;

namespace FModel.Methods.Utilities
{
    static class ImagesUtility
    {


        public static Image GetImageSource(Stream stream)
        {
            Bitmap photo = new Bitmap(stream);

            return photo;
        }



        private static int ChangeColorOpacity(int color, int opacity)
        {
            color -= 255 - opacity;

            return AdjustColorValue(color);
        }
        private static int AdjustColorValue(int color)
        {
            if (color > 255)
            {
                color = 255;
            }
            else if (color < 0)
            {
                color = 0;
            }

            return color;
        }




    }
}
