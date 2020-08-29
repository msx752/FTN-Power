using System;
using System.Drawing;

namespace Image.Model.Options
{
    public interface IImageOptions
    {
        void Point(int x, int y, int w, int h);
        Rectangle Position { get;  }
        void Fill(Color color);
        Graphics BaseGraphics { get; }
        void Point(int x, int y);
        void Point(float x, float y);
        int BaseWidth { get; }
        int BaseHeight { get; }
        IImageOptions Text(string text, string fontName, float emSize, FontStyle fontStyle, Action<IImageTextOptions> option);
        IImageOptions Image(int x, int y, int w, int h, Action<IImageOptions> option);
        IImageOptions Text(string text, string fontName, float emSize, FontStyle fontStyle, StringFormat stringFormat, Action<IImageTextOptions> option);
    }
}