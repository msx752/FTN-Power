using Image.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FTNPower.Image.Api.Tests.Others
{
    public class BitmapTests
    {
        [Fact]
        public void Bitmap1()
        {
            var bitmap = Bitraphic.Draw(460, 510, (o) =>
            {
                var backgroundColor = Color.FromArgb(32, 34, 36);
                var titleColor = Color.Snow;
                var columnColor = Color.FromArgb(255, 250, 212);

                o.Fill(backgroundColor);
                SizeF slboard = SizeF.Empty;
                o.Text("Discord Leaderboards", "Anton", 18f, FontStyle.Regular, null, (t) => { t.ForeColor = titleColor; slboard = t.MeasureString; t.Point(((t.BaseWidth / 2) - (slboard.Width / 2)), 2); });
                SizeF snum = SizeF.Empty;
                o.Text("#", "Anton", 12f, FontStyle.Regular, (t) =>
                {
                    t.ForeColor = columnColor; snum = t.MeasureString; t.Point(5, slboard.Height);
                });
                o.Text("Epic Name", "Anton", 12f, FontStyle.Regular, (t) =>
                {
                    t.ForeColor = columnColor; t.Point(57, slboard.Height);
                });
                o.Text("PL", "Anton", 12f, FontStyle.Regular, (t) =>
                {
                    t.ForeColor = columnColor; t.Point(210, slboard.Height);
                });
                o.Text("Commander Lv", "Anton", 12f, FontStyle.Regular, (t) =>
                {
                    t.ForeColor = columnColor; t.Point(260, slboard.Height);
                });
                o.Text("Collection Lv", "Anton", 12f, FontStyle.Regular, (t) =>
                {
                    t.ForeColor = columnColor; t.Point(365, slboard.Height);
                });

                var lineHeight = (slboard.Height + snum.Height) - 5;
                for (int i = 0; i < 20; i++)
                {
                    o.Image(0, (int)lineHeight, 460, 30, (img1) =>
                    {
                        SizeF sNum = Size.Empty;
                        SizeF sName = Size.Empty;
                        var sf = new StringFormat(StringFormatFlags.NoWrap)
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };
                        img1.Text($"{(i + 1)}", "Anton", 12f, FontStyle.Regular, (t) =>
                         {
                             t.ForeColor = titleColor; sNum = t.MeasureString; t.Point(5, 0);
                         });
                        img1.Text("AbbbbbbbbbbbbbbbbbbbC", "Arial", 11f, FontStyle.Regular, sf, (t) =>
                        {
                            sName = t.MeasureString;
                            t.ForeColor = titleColor;
                            t.Point(25, -3, 180, img1.BaseHeight);
                        });
                        img1.Text("131.17", "Anton", 12f, FontStyle.Regular, (t) =>
                        {
                            t.ForeColor = titleColor; t.Point(202, 0, 48, img1.BaseHeight);
                        });

                        img1.Text((1234).ToString(), "Anton", 12f, FontStyle.Regular, (t) =>
                        {
                            t.ForeColor = titleColor; t.Point(290, 0, 50, img1.BaseHeight);
                        });

                        img1.Text((5678).ToString(), "Anton", 12f, FontStyle.Regular, (t) =>
                        {
                            t.ForeColor = titleColor; t.Point(390, 0, 50, img1.BaseHeight);
                        });

                        lineHeight += (sNum.Height - 4);
                    });
                }
            });
            bitmap.Save(Path.Combine(Directory.GetCurrentDirectory(), "discord-leaderboards.png"));
        }
    }
}
