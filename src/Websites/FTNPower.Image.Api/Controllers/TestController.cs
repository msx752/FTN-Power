using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FTNPower.Image.Processing;
using FTNPower.Model.Tables.StoredProcedures;
using Image.Core;
using Microsoft.AspNetCore.Mvc;

namespace FTNPower.Image.Api.Controllers
{
    [Route("api/Test")]
    public class TestController : CommonController
    {
        readonly ImageProcessorQ imgProcq = null;
        public TestController(ImageProcessorQ prsq)
        {
            imgProcq = prsq;
        }

        [Obsolete]
        [HttpGet("Font")]
        public IActionResult Index()
        {
            try
            {
                List<EpicTopUser> lst = new List<EpicTopUser>()
                {
                     new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "ReiS_eFe",
    Id= "",
    EpicId= "bb8d9b82ffff483db918854e48ef2a02",
    CommanderLevel= 3886,
    CollectionBookLevel= 321
  },
 new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "Dksamvener59",
    Id= "",
    EpicId= "557092c9172c437a98c10e19094757a2",
    CommanderLevel= 2474,
    CollectionBookLevel= 840
  },
 new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "BoPe-EaSy-69",
    Id= "",
    EpicId= "ae8c1207d4294640b7337f68cfc8c78e",
    CommanderLevel= 2128,
    CollectionBookLevel= 916
  },
 new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "RiP STW",
    Id= "",
    EpicId= "f0937a00868c4f6aba1cd423b12a483a",
    CommanderLevel= 1933,
    CollectionBookLevel= 853
  },
 new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "Capsulez",
    Id= "",
    EpicId= "7f07b1994ec24353a609bce0c45d9d8d",
    CommanderLevel= 1733,
    CollectionBookLevel= 909
  },
 new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "NOT BACK DOWN",
    Id= "",
    EpicId= "781caa3737e049ccbc67b37de1ce2332",
    CommanderLevel= 1727,
    CollectionBookLevel= 913
  },
 new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "VashenZ",
    Id= "",
    EpicId= "0d532db2493b42de82948e4f6646c138",
    CommanderLevel= 1642,
    CollectionBookLevel= 620
  },
 new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "ASC-LEEIF",
    Id= "",
    EpicId= "415b65a5579644ee85795b424986f43c",
    CommanderLevel= 1548,
    CollectionBookLevel= 533
  },
 new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "XDark_AuroraX",
    Id= "",
    EpicId= "db801a96ddb747ee9d2e5fda4e39e81e",
    CommanderLevel= 1424,
    CollectionBookLevel= 774
  },
 new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "tooornado.",
    Id= "",
    EpicId= "e91999181df342b8885196776490610f",
    CommanderLevel= 1416,
    CollectionBookLevel= 898
  },
 new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "Biel-ThE-BeSt-69",
    Id= "",
    EpicId= "fd910ea209bd42cb9bb572115906fd9e",
    CommanderLevel= 1395,
    CollectionBookLevel= 306
  },
 new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "4AM-XDD",
    Id= "",
    EpicId= "0820fe741e7240c281259c336382b838",
    CommanderLevel= 1374,
    CollectionBookLevel= 460
  },
 new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "lCarlo1231",
    Id= "",
    EpicId= "4d7aa8e9d9d24622844813f684c8bd63",
    CommanderLevel= 1329,
    CollectionBookLevel= 353
  },
 new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "ᴺᴱᵂwhitebeard",
    Id= "",
    EpicId= "b1dae39a75fc415488334b4f9e379d3a",
    CommanderLevel= 1301,
    CollectionBookLevel= 473
  },
 new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "iTymeStopper",
    Id= "",
    EpicId= "f4c788af16114bd9bf47c4c4981216fa",
    CommanderLevel= 1286,
    CollectionBookLevel= 550
  },
 new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "SirEDGE1987",
    Id= "",
    EpicId= "bc603a56a79f45c7a49a8b3a241469cf",
    CommanderLevel= 1276,
    CollectionBookLevel= 607
  },
 new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "destolution",
    Id= "",
    EpicId= "4421b910c16b49359714ebeb6780faed",
    CommanderLevel= 1275,
    CollectionBookLevel= 780
  },
 new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "CAB-77_",
    Id= "",
    EpicId= "9e3cbb0900614d9c917505e85ba0d578",
    CommanderLevel= 1273,
    CollectionBookLevel= 391
  },
 new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "Yellow Robot",
    Id= "",
    EpicId= "5e04f1c9dd7d4730a5d7ff162f8ad9ee",
    CommanderLevel= 1264,
    CollectionBookLevel= 535
  },
 new EpicTopUser(){
    AccountPowerLevel= 131.17,
    PlayerName= "ارنووووب",
    Id= "",
    EpicId= "cff8daf786364f28b50e861993d0c16a",
    CommanderLevel= 1258,
    CollectionBookLevel= 338
  }
                };

                var intro = "Discord";
                System.Drawing.Image bitmap = Bitraphic.Draw(460, 510, (o) =>
                {
                    var backgroundColor = System.Drawing.Color.FromArgb(32, 34, 36);
                    var titleColor = System.Drawing.Color.Snow;
                    var columnColor = System.Drawing.Color.FromArgb(255, 250, 212);

                    o.Fill(backgroundColor);
                    System.Drawing.SizeF slboard = System.Drawing.SizeF.Empty;
                    o.Text($"{intro} Leaderboards", "Anton", 18f, System.Drawing.FontStyle.Regular, null, (t) => { t.ForeColor = titleColor; slboard = t.MeasureString; t.Point(((t.BaseWidth / 2) - (slboard.Width / 2)), 2); });
                    System.Drawing.SizeF snum = System.Drawing.SizeF.Empty;
                    o.Text("#", "Anton", 12f, System.Drawing.FontStyle.Regular, (t) =>
                    {
                        t.ForeColor = columnColor; snum = t.MeasureString; t.Point(5, slboard.Height);
                    });
                    o.Text("Epic Name", "Anton", 12f, System.Drawing.FontStyle.Regular, (t) =>
                    {
                        t.ForeColor = columnColor; t.Point(57, slboard.Height);
                    });
                    o.Text("PL", "Anton", 12f, System.Drawing.FontStyle.Regular, (t) =>
                    {
                        t.ForeColor = columnColor; t.Point(210, slboard.Height);
                    });
                    o.Text("Commander Lv", "Anton", 12f, System.Drawing.FontStyle.Regular, (t) =>
                    {
                        t.ForeColor = columnColor; t.Point(260, slboard.Height);
                    });
                    o.Text("Collection Lv", "Anton", 12f, System.Drawing.FontStyle.Regular, (t) =>
                    {
                        t.ForeColor = columnColor; t.Point(365, slboard.Height);
                    });

                    var lineHeight = (slboard.Height + snum.Height) - 5;
                    for (int i = 0; i < lst.Count; i++)
                    {
                        EpicTopUser u = lst[i];
                        o.Image(0, (int)lineHeight, 460, 30, (img1) =>
                        {
                            System.Drawing.SizeF sNum = System.Drawing.Size.Empty;
                            System.Drawing.SizeF sName = System.Drawing.Size.Empty;
                            var sf = new System.Drawing.StringFormat(System.Drawing.StringFormatFlags.NoWrap)
                            {
                                Alignment = System.Drawing.StringAlignment.Center,
                                LineAlignment = System.Drawing.StringAlignment.Center
                            };
                            img1.Text($"{((double)i + (double)1).ToString("00")}", "Anton", 12f, System.Drawing.FontStyle.Regular, (t) =>
                            {
                                t.ForeColor = titleColor; sNum = t.MeasureString; t.Point(5, 0);
                            });
#if DEBUG
                            img1.Text(u.PlayerName, "Arial", 11f, System.Drawing.FontStyle.Regular, sf, (t) =>
#else

                            img1.Text(u.PlayerName, "Ubuntu", 11f, System.Drawing.FontStyle.Regular, sf, (t) =>
#endif
                            {
                            sName = t.MeasureString;
                                t.ForeColor = titleColor;
                                t.Point(25, -3, 180, img1.BaseHeight);
                            });
                            img1.Text(u.AccountPowerLevel.ToString("0.00"), "Anton", 12f, System.Drawing.FontStyle.Regular, (t) =>
                            {
                                t.ForeColor = titleColor; t.Point(202, 0, 48, img1.BaseHeight);
                            });

                            img1.Text(u.CommanderLevel.ToString("0,000").TrimStart(new char[] { '0', ',' }), "Anton", 12f, System.Drawing.FontStyle.Regular, (t) =>
                            {
                                t.ForeColor = titleColor; t.Point(290, 0, 50, img1.BaseHeight);
                            });

                            img1.Text(u.CollectionBookLevel.ToString("0,000").TrimStart(new char[] { '0', ',' }), "Anton", 12f, System.Drawing.FontStyle.Regular, (t) =>
                            {
                                t.ForeColor = titleColor; t.Point(390, 0, 50, img1.BaseHeight);
                            });

                            lineHeight += (sNum.Height - 4);
                        });
                    }
                });

                return File(Bitraphic.SaveToStream(bitmap), "image/png");
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}