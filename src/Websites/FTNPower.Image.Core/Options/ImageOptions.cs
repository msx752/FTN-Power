using Image.Model.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image.Core.Options
{
    internal class ImageOptions : IImageOptions
    {
        internal ImageOptions()
        {

        }
        public Graphics BaseGraphics { get; private set; }
        public ImageOptions(System.Drawing.Image img, Graphics graphics)
        {
            BaseGraphics = graphics;
            BaseImage = img;
        }
        internal System.Drawing.Image BaseImage { get; private set; }
        public void Point(int x, int y)
        {
            Position = new Rectangle(x, y, 0, 0);
        }
        public void Point(float x, float y)
        {
            Point((int)x, (int)y);
        }
        public void Point(int x, int y, int w, int h)
        {
            Position = new Rectangle(x, y, w, h);
        }
        public Rectangle Position { get; private set; }
        public void Fill(Color color)
        {
            BaseGraphics.Clear(color);
        }

        public IImageOptions Image(int x, int y, int w, int h, Action<IImageOptions> option)
        {
            var Img = Bitraphic.Draw(w, h, option);
            BaseGraphics.DrawImage(Img, new Rectangle(x, y, w, h));
            return this;
        }
        public IImageOptions Text(string text, string fontName, float emSize, FontStyle fontStyle, Action<IImageTextOptions> option)
        {
            var opt = new ImageTextOptions(this.BaseImage, this.BaseGraphics);
            opt.String(text, fontName, emSize, fontStyle, null);
            option(opt);
            opt.BaseGraphics.DrawString(opt.TextValue, opt.Font, new SolidBrush(opt.ForeColor), opt.Position, opt.Format);
            return this;
        }
        public IImageOptions Text(string text, string fontName, float emSize, FontStyle fontStyle, StringFormat stringFormat, Action<IImageTextOptions> option)
        {
            var opt = new ImageTextOptions(this.BaseImage, this.BaseGraphics);
            opt.String(text, fontName, emSize, fontStyle, stringFormat);
            option(opt);
            opt.BaseGraphics.DrawString(opt.TextValue, opt.Font, new SolidBrush(opt.ForeColor), opt.Position, opt.Format);
            return this;
        }

        public int BaseWidth { get => BaseImage.Width; }
        public int BaseHeight { get => BaseImage.Height; }
    }
}
