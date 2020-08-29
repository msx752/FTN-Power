using FTNPower.Image.Api.Service;
using FTNPower.Image.Api.Service.Models;
using FTNPower.Image.Processing.Models;
using FTNPower.Image.Processing.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace FTNPower.Image.Processing
{
    public class ImageProcessorQ
    {
        private Color dark2 = Color.FromArgb(32, 34, 37);
        private ImageService _isvc { get => ImageProvider.ISVC; }

        [Obsolete]
        public Stream Draw_StwStore2(StwStoreModel model)
        {
            int boxArea = 260;

            var left = DrawViewArea(model.STWSpecialEventStorefront, boxArea);
            var right = DrawViewArea(model.STWRotationalEventStorefront, boxArea);
            model = null;
            Bitmap leftBitmap = PlaceAllImages2(left, boxArea);
            left.Clear();
            Bitmap rightBitmap = PlaceAllImages2(right, boxArea);
            right.Clear();

            int h = leftBitmap.Height;
            if (rightBitmap.Height > h)
                h = rightBitmap.Height;

            int topMargin = 50;
            Stream rstream = new MemoryStream();
            rstream = OpenGraphics((leftBitmap.Width + rightBitmap.Width), (h + topMargin), dark2, (b, g) =>
            {
                int leftWidth = leftBitmap.Width;
                int rightWidth = rightBitmap.Width;
                g.DrawImage(leftBitmap, 0, topMargin);
                SizeF featuredQuantity = g.MeasureString($"Featured", new Font("Arial Black", 39f, FontStyle.Bold));
                g.DrawString($"Featured", new Font("Arial Black", 39f, FontStyle.Bold), Brushes.WhiteSmoke, ((leftWidth / 2) - (featuredQuantity.Width / 2)), 3);
                leftBitmap.Dispose();

                g.DrawImage(rightBitmap, leftWidth, topMargin);
                SizeF weeklyQuantity = g.MeasureString($"Weekly", new Font("Arial Black", 39f, FontStyle.Bold));
                g.DrawString($"Weekly", new Font("Arial Black", 39f, FontStyle.Bold), Brushes.WhiteSmoke, ((((leftWidth) / 2) + rightWidth) - (weeklyQuantity.Width / 2)) + 10, 3);
                rightBitmap.Dispose();

                SizeF ftnpowerQuality = g.MeasureString("www.ftnpower.com", new Font("Arial Black", 29f, FontStyle.Bold));
                g.DrawString("www.ftnpower.com", new Font("Arial Black", 29f, FontStyle.Bold), Brushes.Yellow, ((leftWidth + rightWidth) / 2) - (ftnpowerQuality.Width / 2), 0);
            });
            return rstream;
        }

        private Bitmap DrawItemViewArea(int boxArea, AssetDat i, RawApiRequestItem item)
        {
            var itemView = OpenBitmap(boxArea, boxArea, null, (b, g) =>
            {
                int x = 3, y = 3, w = 6, h = 6;

                #region rarity

                Bitmap rarity = null;
                switch (i.GetRarity())
                {
                    case 1:
                        rarity = Properties.Resources.Common;
                        g.Clear(Color.FromArgb(183, 183, 183));
                        break;

                    case 2:
                        rarity = Properties.Resources.UnCommon;
                        g.Clear(Color.FromArgb(183, 183, 183));
                        break;

                    case 3:
                        rarity = Properties.Resources.Rare;
                        g.Clear(Color.FromArgb(42, 187, 249));
                        break;

                    case 4:
                        rarity = Properties.Resources.Epic;
                        g.Clear(Color.FromArgb(192, 88, 252));
                        break;

                    case 5:
                        rarity = Properties.Resources.Legendary;
                        g.Clear(Color.FromArgb(231, 139, 45));
                        break;

                    case 6:
                        rarity = Properties.Resources.Mythic;
                        g.Clear(Color.FromArgb(255, 224, 160));
                        break;

                    default:
                        rarity = Properties.Resources.UnCommon;
                        g.Clear(Color.FromArgb(183, 183, 183));
                        break;
                }
                if (rarity != null)
                    g.DrawImage(rarity, new Rectangle(x, y, b.Width - w, b.Width - h));

                #endregion rarity

                #region draw image

                var imPath = _isvc.GetImagePhysLocation(i.Id);
                g.DrawImage(Bitmap.FromFile(imPath), new Rectangle(x, y, b.Width - w, b.Width - h));

                #endregion draw image

                #region transparentArea & displayName & Price

                int text_x = x, text_y = (b.Height - (int)(b.Height * 0.3) - y), text_w = b.Width - (x * 2), text_h = (int)(b.Height * 0.3);
                g.DrawImage(DrawTransparentArea2(text_w, text_h, 0.75f), text_x, text_y);

                SizeF sizeName = g.MeasureString(i.DisplayName, new Font("Arial Black", 14f, FontStyle.Bold));
                int nameLeft = Convert.ToInt32((text_w / 2) - (sizeName.Width / 2));
                g.DrawString(i.DisplayName, new Font("Arial Black", 14f, FontStyle.Bold), Brushes.WhiteSmoke, new PointF(nameLeft, text_y));

                SizeF sizeGold = g.MeasureString(item.Price.Value.ToString("N0"), new Font("Arial Black", 24f, FontStyle.Bold));
                int vbuckLeft = Convert.ToInt32((text_w / 2) - (sizeGold.Width / 2)) + 10;
                g.DrawString(item.Price.Value.ToString("N0"), new Font("Arial Black", 24f, FontStyle.Bold), Brushes.WhiteSmoke, new PointF(vbuckLeft, text_y + 30));
                g.DrawImage(Resources.eventscaling, vbuckLeft - 40, text_y + 30, 44, 44);

                #endregion transparentArea & displayName & Price

                #region quantity

                int quantity = item.weeklyLimit.HasValue ? item.weeklyLimit.Value : 0;
                if (quantity == -1)
                {
                    if (!string.IsNullOrWhiteSpace(item.EventLimit))
                    {
                        if (!int.TryParse(item.EventLimit, out quantity))
                            quantity = -1;
                    }
                }
                if (quantity > 1)
                {
                    SizeF sizeQuantity = g.MeasureString($"x{quantity.ToString()}", new Font("Arial Black", 24f, FontStyle.Bold));
                    g.DrawImage(DrawTransparentArea2(((int)sizeQuantity.Width), 45, 0.75f), (b.Width - (int)(sizeQuantity.Width + (x * 1))), y + 1);
                    g.DrawString($"x{quantity.ToString()}", new Font("Arial Black", 24f, FontStyle.Bold), Brushes.WhiteSmoke, (b.Width - (int)(sizeQuantity.Width + x)) + 1, y);
                }

                #endregion quantity
            });
            return itemView;
        }

        private Bitmap DrawTransparentArea2(int w, int h, float opacity = 0.71f)
        {
            using (Bitmap image = (Bitmap)new Bitmap(w, h))
            {
                using (Graphics gr2 = image.ToGraphics())
                {
                    gr2.Clear(Color.Black);
                }
                Bitmap bmp = new Bitmap(image.Width, image.Height);
                using (Graphics gfx = bmp.ToGraphics())
                {
                    ColorMatrix matrix = new ColorMatrix();
                    matrix.Matrix33 = opacity;
                    ImageAttributes attributes = new ImageAttributes();
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
                return bmp;
            }
        }

        [Obsolete]
        private List<Bitmap> DrawViewArea(List<RawApiRequestItem> items, int boxArea)
        {
            var lst = _isvc
                .GetImageDatasByIds(items.Select(x => x.templateId).ToArray())
                .OrderByDescending(f => f.GetRarity())
                .ThenByDescending(f => f.IsHero())
                .ThenByDescending(f => f.IsWeapon())
                .ThenByDescending(f => f.IsDefender())
                .ToList();
            List<Bitmap> b = new List<Bitmap>();
            foreach (var i in lst)
            {
                try
                {
                    var item = items.FirstOrDefault(f => f.templateId.EndsWith(i.Id, StringComparison.InvariantCultureIgnoreCase));
                    Bitmap itemView = DrawItemViewArea(boxArea, i, item);
                    b.Add(itemView);
                    //break
                }
                catch (Exception e)
                {
                    continue;
                }
            }
            lst = null;
            return b;
        }

        private Bitmap OpenBitmap(int w, int h, Color? backColor, Action<Bitmap, Graphics> myaction)
        {
            Bitmap b = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            using (Graphics gr = b.ToGraphics())
            {
                if (backColor.HasValue)
                    gr.Clear(backColor.Value);
                myaction(b, gr);
            }
            return b;
        }

        private Stream OpenGraphics(int w, int h, Color? backColor, Action<Bitmap, Graphics> myaction)
        {
            Stream rstream = new MemoryStream();
            using (Bitmap b = OpenBitmap(w, h, backColor, myaction))
            {
                Extensions.Save(b, rstream);
            }
            return rstream;
        }

        private Bitmap PlaceAllImages2(List<Bitmap> Items, int boxArea)
        {
            int margin = 20, RowCounter = 0, amountOfRowItems = 3, onePanelSize = 860;
            return OpenBitmap(onePanelSize, (((Items.Count / amountOfRowItems) + 1) * (boxArea + margin)) + margin, dark2, (b, g) =>
            {
                int x = margin, y = margin;
                for (int i = 0; i < Items.Count; i = (i + 3))
                {
                    if ((i + amountOfRowItems) > Items.Count)
                        RowCounter = Items.Count;
                    else
                        RowCounter = (i + amountOfRowItems);

                    for (int i2 = i; i2 < RowCounter; i2++)
                    {
                        using (Bitmap bit = Items[i2])
                        {
                            g.DrawImage(bit, x, y, boxArea, boxArea);
                            x += margin;
                            x += boxArea;
                        }
                    }
                    y += margin;
                    y += boxArea;
                    x = margin;
                }
            });
        }
    }
}