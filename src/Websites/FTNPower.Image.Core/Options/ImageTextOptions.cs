using Image.Model.Options;
using System;
using System.Drawing;

namespace Image.Core.Options
{
    internal class ImageTextOptions : ImageOptions, IImageTextOptions
    {
        internal ImageTextOptions()
        {

        }
        internal ImageTextOptions(System.Drawing.Image img, Graphics graphics) : base(img, graphics)
        {
        }

        internal void String(string text, string fontName, float emSize, FontStyle fontStyle, StringFormat? stringFormat = null)
        {
            TextValue = text;
            Format = stringFormat;
            Font = Bitraphic.GetFont(fontName, emSize, fontStyle);
            MeasureString = BaseGraphics.MeasureString(text, Font);
        }
        ///add pointmethod for the StringFormat parameter
        public StringFormat? Format { get; private set; }
        public Font Font { get; private set; }
        public SizeF MeasureString { get; private set; }
        public string TextValue { get; private set; }
        /// <summary>
        /// (default: black)
        /// </summary>
        public Color ForeColor { get; set; } = Color.Black;
    }
}
