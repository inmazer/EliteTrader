using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using EliteTrader;
using EliteTrader.EliteOcr;
using EliteTrader.EliteOcr.Data;
using EliteTrader.EliteOcr.Tesseract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThruddClient;
using CommodityItem = ThruddClient.CommodityItem;

namespace EliteOcrTests
{
    [TestClass]
    [DeploymentItem(@"TestProjectFolder\tessdata", "tessdata")]
    [DeploymentItem(@"TestProjectFolder\x86", "x86")]
    [DeploymentItem(@"TestProjectFolder\x64", "x64")]
    public class Prepper : TestScreenshotBase
    {
        //[TestMethod]
        //public void PrintDictionary()
        //{
        //    TessHelper.UpdateDictionary(@"C:\Git\EliteTrader\EliteOcr\tessdata\edl.user-words", new CommodityItemNameMatcher(), new StationNameRepository());
        //}

        //[TestMethod]
        //public void Tore()
        //{
        //    foreach (string str in DetfaultRareCommodities.RareNames)
        //    {
        //        Console.WriteLine(str);
        //    }
        //}

        //[TestMethod]
        [DeploymentItem(@"Screenshots\McCoolCity\Screenshot_0000.bmp")]
        public void PrintNewTrainingFile()
        {
            const string location = @"c:\tmp\screens\tmp";

            ParsedScreenshotBitmaps parsedScreenshotBitmaps = ImageParser.Parse(new Bitmap(@"Screenshot_0000.bmp"));

            //TiffSaver.SaveScaled(location, parsedScreenshotBitmaps.Name, 225);
            //TiffSaver.SaveScaled(location, parsedScreenshotBitmaps.Name, 150);
            TiffSaver.Save(location, parsedScreenshotBitmaps.Name);

            TiffSaver.Save(location, parsedScreenshotBitmaps.ItemBitmapsList[12].Name.Bitmap);

            //TiffSaver.SaveScaled(location, parsedScreenshotBitmaps.Description, 225);
            //TiffSaver.SaveScaled(location, parsedScreenshotBitmaps.Description, 150);
            //TiffSaver.Save(location, parsedScreenshotBitmaps.Description);

            //foreach (CommodityItemBitmaps item in parsedScreenshotBitmaps.ItemBitmapsList)
            //{
            //    //TiffSaver.SaveScaled(location, item.Name.Bitmap, 200);
            //    //TiffSaver.SaveScaled(location, item.Name.Bitmap, 150);
            //    //TiffSaver.Save(location, item.Name.Bitmap);
            //    //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 225);
            //    //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 150);
            //    //TiffSaver.Save(location, item.Sell.Bitmap);

            //    //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 225);
            //    //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 150);
            //    //TiffSaver.Save(location, item.Buy.Bitmap);
            //    //TiffSaver.Save(location, item.Supply.Bitmap);
            //    //TiffSaver.Save(location, item.Demand.Bitmap);
            //    //TiffSaver.Save(location, item.GalacticAverage.Bitmap);
            //}

            //parsedScreenshotBitmaps = ImageParser.Parse(new Bitmap(@"Screenshot_0001.bmp"));
            //foreach (CommodityItemBitmaps item in parsedScreenshotBitmaps.ItemBitmapsList)
            //{
            //    //TiffSaver.Save(location, item.Name.Bitmap);

            //    //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 225);
            //    //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 150);
            //    //TiffSaver.Save(location, item.Sell.Bitmap);

            //    //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 225);
            //    //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 150);
            //    //TiffSaver.Save(location, item.Buy.Bitmap);
            //}

            //parsedScreenshotBitmaps = ImageParser.Parse(new Bitmap(@"Screenshot_0002.bmp"));
            //foreach (CommodityItemBitmaps item in parsedScreenshotBitmaps.ItemBitmapsList)
            //{
            //    //TiffSaver.Save(location, item.Name.Bitmap);

            //    //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 225);
            //    //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 150);
            //    //TiffSaver.Save(location, item.Sell.Bitmap);

            //    //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 225);
            //    //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 150);
            //    //TiffSaver.Save(location, item.Buy.Bitmap);
            //}

            //parsedScreenshotBitmaps = ImageParser.Parse(new Bitmap(@"Screenshot_0003.bmp"));
            //TiffSaver.Save(location, parsedScreenshotBitmaps.ItemBitmapsList[3].Name.Bitmap);
            
            //foreach (CommodityItemBitmaps item in parsedScreenshotBitmaps.ItemBitmapsList)
            //{
            //    //TiffSaver.Save(location, item.Name.Bitmap);

            //    //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 225);
            //    //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 150);
            //    TiffSaver.Save(location, item.Sell.Bitmap);

            //    //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 225);
            //    //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 150);
            //    TiffSaver.Save(location, item.Buy.Bitmap);
            //}

            TiffSaver.FlushTiff();
        }

        //[TestMethod]
        //[DeploymentItem(@"Screenshots\FortKlarix\0.bmp")]
        //[DeploymentItem(@"Screenshots\FortKlarix\1.bmp")]
        //[DeploymentItem(@"Screenshots\FortKlarix\2.bmp")]
        //[DeploymentItem(@"Screenshots\FortKlarix\3.bmp")]
        //[DeploymentItem(@"Screenshots\FortKlarix\4.bmp")]
        //[DeploymentItem(@"Screenshots\FortKlarix\5.bmp")]
        //public void PrintNewTrainingFile()
        //{
        //    const string location = @"c:\tmp\screens\tmp";

        //    ParsedScreenshotBitmaps parsedScreenshotBitmaps = ImageParser.Parse(new Bitmap(@"0.bmp"));

        //    //TiffSaver.SaveScaled(location, parsedScreenshotBitmaps.Name, 225);
        //    //TiffSaver.SaveScaled(location, parsedScreenshotBitmaps.Name, 150);
        //    //TiffSaver.Save(location, parsedScreenshotBitmaps.Name);

        //    //TiffSaver.SaveScaled(location, parsedScreenshotBitmaps.Description, 225);
        //    //TiffSaver.SaveScaled(location, parsedScreenshotBitmaps.Description, 150);
        //    //TiffSaver.Save(location, parsedScreenshotBitmaps.Description);

        //    foreach (CommodityItemBitmaps item in parsedScreenshotBitmaps.ItemBitmapsList)
        //    {
        //        //TiffSaver.SaveScaled(location, item.Name.Bitmap, 200);
        //        //TiffSaver.SaveScaled(location, item.Name.Bitmap, 150);
        //        //TiffSaver.Save(location, item.Name.Bitmap);
        //        //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 225);
        //        //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 150);
        //        TiffSaver.Save(location, item.Sell.Bitmap);

        //        //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 225);
        //        //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 150);
        //        TiffSaver.Save(location, item.Buy.Bitmap);
        //        //TiffSaver.Save(location, item.Supply.Bitmap);
        //        //TiffSaver.Save(location, item.Demand.Bitmap);
        //        //TiffSaver.Save(location, item.GalacticAverage.Bitmap);
        //    }

        //    parsedScreenshotBitmaps = ImageParser.Parse(new Bitmap(@"1.bmp"));
        //    foreach (CommodityItemBitmaps item in parsedScreenshotBitmaps.ItemBitmapsList)
        //    {
        //        //TiffSaver.Save(location, item.Name.Bitmap);

        //        //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 225);
        //        //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 150);
        //        TiffSaver.Save(location, item.Sell.Bitmap);

        //        //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 225);
        //        //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 150);
        //        TiffSaver.Save(location, item.Buy.Bitmap);
        //    }

        //    parsedScreenshotBitmaps = ImageParser.Parse(new Bitmap(@"2.bmp"));
        //    foreach (CommodityItemBitmaps item in parsedScreenshotBitmaps.ItemBitmapsList)
        //    {
        //        //TiffSaver.Save(location, item.Name.Bitmap);

        //        //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 225);
        //        //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 150);
        //        TiffSaver.Save(location, item.Sell.Bitmap);

        //        //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 225);
        //        //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 150);
        //        TiffSaver.Save(location, item.Buy.Bitmap);
        //    }

        //    parsedScreenshotBitmaps = ImageParser.Parse(new Bitmap(@"3.bmp"));
        //    foreach (CommodityItemBitmaps item in parsedScreenshotBitmaps.ItemBitmapsList)
        //    {
        //        //TiffSaver.Save(location, item.Name.Bitmap);

        //        //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 225);
        //        //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 150);
        //        TiffSaver.Save(location, item.Sell.Bitmap);

        //        //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 225);
        //        //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 150);
        //        TiffSaver.Save(location, item.Buy.Bitmap);
        //    }

        //    parsedScreenshotBitmaps = ImageParser.Parse(new Bitmap(@"4.bmp"));
        //    foreach (CommodityItemBitmaps item in parsedScreenshotBitmaps.ItemBitmapsList)
        //    {
        //        //TiffSaver.Save(location, item.Name.Bitmap);

        //        //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 225);
        //        //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 150);
        //        TiffSaver.Save(location, item.Sell.Bitmap);

        //        //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 225);
        //        //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 150);
        //        TiffSaver.Save(location, item.Buy.Bitmap);
        //    }

        //    parsedScreenshotBitmaps = ImageParser.Parse(new Bitmap(@"5.bmp"));
        //    foreach (CommodityItemBitmaps item in parsedScreenshotBitmaps.ItemBitmapsList)
        //    {
        //        //TiffSaver.Save(location, item.Name.Bitmap);

        //        //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 225);
        //        //TiffSaver.SaveScaled(location, item.Sell.Bitmap, 150);
        //        TiffSaver.Save(location, item.Sell.Bitmap);

        //        //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 225);
        //        //TiffSaver.SaveScaled(location, item.Buy.Bitmap, 150);
        //        TiffSaver.Save(location, item.Buy.Bitmap);
        //    }

        //    TiffSaver.FlushTiff();
        //}

        
    }
}
