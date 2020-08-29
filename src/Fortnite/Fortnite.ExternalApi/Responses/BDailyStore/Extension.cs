using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Fortnite.External.Responses.BDailyStore
{
    public static class Extension
    {
        public static string GetBrDailyTime(this BrDailyStore brStore)
        {
            try
            {
                var LastUpdateTime = DateTimeOffset.UtcNow;
                return LastUpdateTime.ToString("dd/MMMM/yyyy");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetBrDailyTitle(this BrDailyStore brStore)
        {
            try
            {
                var LastUpdateTime = DateTimeOffset.UtcNow;
                //Fortnite: **Battle Royale Daily Store** [ *12 May 2019* ]
                return $"**Battle Royale Store** [ *{LastUpdateTime.ToString("dd MMMM yyyy")}* ]";
            }

            catch (Exception e)

            {
                return null;
            }
        }

        public static string GetBrDailyImageName(this BrDailyStore brStore)
        {
            return $"{nameof(BrDailyStore)}.png";
        }

        public async static Task<Bitmap> GetBrDailyImageAsync(this BrDailyStore brStore)
        {
            var cultureInfo = new CultureInfo("en-GB", true);
            cultureInfo.NumberFormat.CurrencySymbol = "£";
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
            var grpLst = brStore.data.GroupBy(f => f.store.isFeatured).ToList();
            Dictionary<bool, Image> Windows = new Dictionary<bool, Image>();
            var vbuckIcon = await GetImage("https://cdn.discordapp.com/emojis/523870428192571402.png?v=1");
            foreach (var item in grpLst)
            {
                var img = await GenerateWindowAsync(item, vbuckIcon);
                Windows.Add(item.Key, img);
            }
            int x = 0, y = 50;
            int tW = Windows.Sum(f => f.Value.Width) + 10;
            int tH = Windows.Max(f => f.Value.Height) + y;
            Bitmap btm = new Bitmap(tW, tH, PixelFormat.Format32bppRgb);
            btm.MakeTransparent(btm.GetPixel(0, 0));
            using (Graphics gr = Graphics.FromImage(btm))
            {
                gr.Clear(Color.FromArgb(54, 57, 63));
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                SolidBrush solidBrush = new SolidBrush(Color.White);
                Font font = new Font("Arial Black", 29f, FontStyle.Bold);
                foreach (var window in Windows)
                {
                    string Title = window.Key == true ? "FEATURED" : "DAILY";
                    gr.DrawImage(window.Value, x, y);
                    using (StringFormat sf = new StringFormat())
                    {
                        sf.Alignment = StringAlignment.Center;
                        Point p = new Point(btm.Width / 2 / 2, 3);
                        if (window.Key == false)
                        {
                            p = new Point(btm.Width / 2 + btm.Width / 2 / 2, 3);
                        }
                        gr.DrawString(Title, font, solidBrush, p, sf);
                    }
                    x += window.Value.Width + 20;
                }
            }
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, brStore.GetBrDailyImageName());
            btm.Save(filePath, ImageFormat.Png);
            return btm;
        }

        private async static Task<Image> GenerateWindowAsync(IGrouping<bool, Datum> items, Bitmap vbuckIcon)
        {
            int boxSize = 300;
            int margin = 10;
            int xline = 2;
            int yline = (int)Math.Round(items.Count() / (double)2, MidpointRounding.AwayFromZero);
            int bw = xline * boxSize + xline * margin;
            int bh = yline * boxSize + yline * margin;
            Bitmap btm = new Bitmap(bw, bh, PixelFormat.Format32bppRgb);
            using (Graphics gr = Graphics.FromImage(btm))
            {
                gr.Clear(Color.FromArgb(54, 57, 63));
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                int x = 0,
                y = margin;
                int i = 0;
                foreach (var grpItem in items)
                {
                    Bitmap avatar = await GetImage(grpItem.item, vbuckIcon, grpItem.store.cost);
                    if (i % xline == 0 & i != 0)
                    {
                        y += boxSize;
                        y += margin;
                        x = 0;
                    }
                    gr.DrawImage(avatar, x, y, boxSize, boxSize);
                    x += boxSize;
                    x += margin;
                    i++;
                }
            }
            return btm;
        }

        public static Image SetImageOpacity(Image image, float opacity)
        {
            try
            {
                //create a Bitmap the size of the image provided
                Bitmap bmp = new Bitmap(image.Width, image.Height);

                //create a graphics object from the image
                Graphics gfx = Graphics.FromImage(bmp);

                //create a color matrix object
                ColorMatrix matrix = new ColorMatrix();

                //set the opacity
                matrix.Matrix33 = opacity;

                //create image attributes
                ImageAttributes attributes = new ImageAttributes();

                //set the color(opacity) of the image
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                //now draw the image
                gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                gfx.Dispose();
                return bmp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async static Task<Bitmap> GetImage(Item item, Bitmap vbuckIcon, string cost)
        {
            var cultureInfo = new CultureInfo("en-GB", true);
            cultureInfo.NumberFormat.CurrencySymbol = "£";
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
            var bgImg = await GetImage(item.images.background);
            using (Graphics gr = Graphics.FromImage(bgImg))
            {
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                var bg = (Image)new Bitmap(492, 122);
                using (Graphics gr2 = Graphics.FromImage(bg))
                {
                    gr2.Clear(Color.Black);
                }
                Image TransparenPanel = SetImageOpacity(bg, 0.71f);
                using (Graphics gr2 = Graphics.FromImage(TransparenPanel))
                {
                    gr2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    gr2.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    SizeF sizeName = gr.MeasureString(item.name, new Font("Arial Black", 31f, FontStyle.Bold));
                    int nameLeft = Convert.ToInt32(TransparenPanel.Width / 2 - sizeName.Width / 2);
                    gr2.DrawString(item.name, new Font("Arial Black", 31f, FontStyle.Bold), Brushes.WhiteSmoke, new PointF(nameLeft, 5));

                    SizeF sizeVbuck = gr.MeasureString(cost, new Font("Arial Black", 28f, FontStyle.Bold));
                    int vbuckLeft = Convert.ToInt32(TransparenPanel.Width / 2 - sizeVbuck.Width / 2);
                    gr2.DrawString(cost, new Font("Arial Black", 28f, FontStyle.Bold), Brushes.WhiteSmoke, new PointF(vbuckLeft, 63));
                    gr2.DrawImage(vbuckIcon, vbuckLeft - 40, 75, 38, 38);
                }
                gr.DrawImage(TransparenPanel, 10, 380);
            }
            return bgImg;
        }

        public static Task<Bitmap> GetImage(string imageUrl)
        {
            return Task.Run(() =>
             {
                 try
                 {
                     using (WebClient client = new WebClient())
                     {
                         Stream stream = client.OpenRead(new Uri(imageUrl));
                         Bitmap bitmap = new Bitmap(stream);
                         return bitmap;
                     }
                 }

                 catch (Exception e)
                 {
                     return new Bitmap(1, 1);
                 }
             });
        }
    }
}