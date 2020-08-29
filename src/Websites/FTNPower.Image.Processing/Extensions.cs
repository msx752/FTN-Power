using ImageMagick;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;

namespace FTNPower.Image.Processing
{
    public static class Extensions
    {
        public static void Save(this Bitmap image, Stream rstream, string mimeType = "image/png")
        {
            System.Drawing.Imaging.ImageCodecInfo codec = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders().First(f => f.MimeType == mimeType);
            System.Drawing.Imaging.EncoderParameters eParams = new System.Drawing.Imaging.EncoderParameters(1);
            eParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
            image.Save(rstream, codec, eParams);
            rstream.Seek(0, SeekOrigin.Begin);
        }
        public static Graphics ToGraphics(this Bitmap image)
        {
            Graphics gr = Graphics.FromImage(image);
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            return gr;
        }

        public static Stream GetStream(this Bitmap b, int w)
        {
            Stream ms = new MemoryStream();
            using (b)
            {
                var _w = b.Width;
                var _h = b.Height;
                if (w != 0)
                {
                    _w = w;
                    _h = b.Height / b.Width * _w;
                    b.GetThumbnailImage(_w, _h, () => false, IntPtr.Zero)
                        .Save(ms, ImageFormat.Png);
                }
                else
                {
                    b.Save(ms, ImageFormat.Png);
                }
            }
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
        public static Stream GetImage(this byte[] bytes, int w)
        {
            Stream ms = new Bitmap(new MemoryStream(bytes, true))
                .GetStream(w);
            return ms;
        }
    }
}
