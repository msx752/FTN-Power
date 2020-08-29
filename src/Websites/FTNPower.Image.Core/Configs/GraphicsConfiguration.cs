using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image.Core.Configs
{
    public class GraphicsConfiguration
    {
        public GraphicsConfiguration()
        {
            SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
        }
        public System.Drawing.Drawing2D.SmoothingMode SmoothingMode { get; set; }
        public System.Drawing.Drawing2D.CompositingQuality CompositingQuality { get; set; }
        public System.Drawing.Drawing2D.InterpolationMode InterpolationMode { get; set; }

    }
}
