using Image.Model.Options;
using System;
using System.Drawing;

namespace Image.Model.Options
{
    public interface IImageTextOptions : IImageOptions
    {
        Font Font { get; }
        Color ForeColor { get; set; }
        StringFormat Format { get; }
        SizeF MeasureString { get; }
        string TextValue { get; }

    }
}