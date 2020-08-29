using Image.Core.Configs;
using Image.Core.Options;
using Image.Model.Options;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Image.Core
{
    public static class Bitraphic
    {
        public static PrivateFontCollection pfc = null;
        public static FontFamily[] GetFonts()
        {
            if (pfc != null) return pfc.Families;
            pfc = new PrivateFontCollection();
            string[] fonts = Directory.GetFiles("CustomFonts", "*.ttf", SearchOption.AllDirectories);
            if (fonts.Length == 0) throw new FileNotFoundException("there is no custom-font to load");
            foreach (var font in fonts)
                pfc.AddFontFile(font);
            return pfc.Families;
        }

        public static System.Drawing.Image Fill(System.Drawing.Image source, Color color)
        {
            using (Graphics gr = source.InitGraphics())
                gr.Clear(color);
            return source;
        }

        public static System.Drawing.Image Transparent(System.Drawing.Image source, float opacity = 1.0f)
        {
            if (!(opacity >= 0.0f && opacity < 1.0f)) return source;
            System.Drawing.Image bmp = new Bitmap(source.Width, source.Height);
            using (Graphics gfx = bmp.InitGraphics())
            {
                ColorMatrix matrix = new ColorMatrix { Matrix33 = opacity };
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                gfx.DrawImage(source, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);
            }
            return bmp;
        }

        //instead of Image we should choice the grapcics
        public static System.Drawing.Image Draw(System.Drawing.Image img, Action<IImageOptions> action)
        {
            using (Graphics gr = img.InitGraphics())
                action(new ImageOptions(img, gr));
            return img;
        }
        public static System.Drawing.Image Draw(int w, int h, Action<IImageOptions> action)
        {
            return Draw(w, h, null, action);
        }
        public static System.Drawing.Image Draw(int w, int h, Color? backColor, Action<IImageOptions> action)
        {
            Bitmap b = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            using (Graphics gr = b.InitGraphics())
            {
                if (backColor.HasValue)
                    gr.Clear(backColor.Value);
                action(new ImageOptions(b, gr));
            }
            return b;
        }

        public static System.Drawing.Image FromUrl(Uri url)
        {
            using (var httpClient = new HttpClient(new HttpClientHandler() { /*.net 5 preview bug bypass*/ ServerCertificateCustomValidationCallback = delegate { return true; } }))
            using (Stream stream = httpClient.GetStreamAsync(url).Result)
                return System.Drawing.Image.FromStream(stream);
        }

        public static System.Drawing.Image OpenGraphic(int w, int h, Color backColor, Action<IImageOptions> myaction)
        {
            System.Drawing.Image b = Draw(w, h, backColor, myaction);
            return b;
        }

        public static Font GetFont(string fontName, float emSize, FontStyle fontStyle)
        {
            FontFamily fnt = GetFonts().FirstOrDefault(f => f.Name.Contains(fontName, StringComparison.InvariantCultureIgnoreCase));
            if (fnt == null)
            {
                return new Font(fontName, emSize, fontStyle);/* "there is no '{fontName}' Font exists in the array."  searching in the computer*/
            }
            return new Font(fnt, emSize, fontStyle);
        }

        public static MemoryStream SaveToStream(System.Drawing.Image source, string mimeType = "image/png", long quality = 100L)
        {
            ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders().First(f => f.MimeType == mimeType);
            EncoderParameters eParams = new EncoderParameters(1);
            eParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            MemoryStream stream = new MemoryStream();
            using (source)
            {
                source.Save(stream, codec, eParams);
                stream.Seek(0, SeekOrigin.Begin);
            }
            return stream;
        }

        internal static Graphics InitGraphics(this System.Drawing.Image source, GraphicsConfiguration grc = null)
        {
            Graphics gr = Graphics.FromImage(source);
            if (grc == null) grc = new GraphicsConfiguration();
            gr.SmoothingMode = grc.SmoothingMode;
            gr.CompositingQuality = grc.CompositingQuality;
            gr.InterpolationMode = grc.InterpolationMode;
            return gr;
        }
    }
}
