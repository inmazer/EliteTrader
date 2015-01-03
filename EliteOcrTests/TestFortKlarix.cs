using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using EliteTrader.EliteOcr;
using EliteTrader.EliteOcr.Enums;
using EliteTrader.EliteOcr.Tesseract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace EliteOcrTests
{
    [TestClass]
    [DeploymentItem(@"TestProjectFolder\tessdata", "tessdata")]
    [DeploymentItem(@"TestProjectFolder\x86", "x86")]
    [DeploymentItem(@"TestProjectFolder\x64", "x64")]
    public class TestFortKlarix : TestScreenshotBase
    {
        [TestMethod]
        [DeploymentItem(@"Screenshots\FortKlarix\0.bmp")]
        [DeploymentItem(@"Screenshots\FortKlarix\0.txt")]
        public void Test0()
        {
            string str = Environment.CurrentDirectory;
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            ParsedScreenshot actual = screenshotParser.Parse(@"0.bmp");

            //Console.WriteLine(actual);
            //string str = JsonConvert.SerializeObject(actual, Formatting.Indented);
            //Console.WriteLine(str);

            ParsedScreenshot expected = JsonConvert.DeserializeObject<ParsedScreenshot>(File.ReadAllText(@"0.txt"));
            Compare(expected, actual); 
        }

        [TestMethod]
        [DeploymentItem(@"Screenshots\FortKlarix\1.bmp")]
        [DeploymentItem(@"Screenshots\FortKlarix\1.txt")]
        public void Test1()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            ParsedScreenshot actual = screenshotParser.Parse(@"1.bmp"); 
            //Console.WriteLine(actual);
            //string str = JsonConvert.SerializeObject(actual, Formatting.Indented);
            //Console.WriteLine(str);

            ParsedScreenshot expected = JsonConvert.DeserializeObject<ParsedScreenshot>(File.ReadAllText(@"1.txt"));
            Compare(expected, actual);
        }

        [TestMethod]
        [DeploymentItem(@"Screenshots\FortKlarix\2.bmp")]
        [DeploymentItem(@"Screenshots\FortKlarix\2.txt")]
        public void Test2()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            ParsedScreenshot actual = screenshotParser.Parse(@"2.bmp");
            //Console.WriteLine(actual);
            //string str = JsonConvert.SerializeObject(actual, Formatting.Indented);
            //Console.WriteLine(str);

            ParsedScreenshot expected = JsonConvert.DeserializeObject<ParsedScreenshot>(File.ReadAllText(@"2.txt"));
            Compare(expected, actual);
        }

        [TestMethod]
        [DeploymentItem(@"Screenshots\FortKlarix\3.bmp")]
        [DeploymentItem(@"Screenshots\FortKlarix\3.txt")]
        public void Test3()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            ParsedScreenshot actual = screenshotParser.Parse(@"3.bmp");

            //Console.WriteLine(actual);
            //string str = JsonConvert.SerializeObject(actual, Formatting.Indented);
            //Console.WriteLine(str);

            ParsedScreenshot expected = JsonConvert.DeserializeObject<ParsedScreenshot>(File.ReadAllText(@"3.txt"));
            Compare(expected, actual);
        }

        //[TestMethod]
        public void PrintNewTrainingFile()
        {
            ParsedScreenshotBitmaps parsedScreenshotBitmaps =
                ImageParser.Parse(new Bitmap(@"screenshots\FortKlarix\3.bmp"));

            const string location = @"c:\tmp\screens\tmp";

            TiffSaver.SaveScaled(location, parsedScreenshotBitmaps.Name, 250);
            TiffSaver.Save(location, parsedScreenshotBitmaps.Name);
            TiffSaver.SaveScaled(location, parsedScreenshotBitmaps.Name, 150);
            TiffSaver.SaveScaled(location, parsedScreenshotBitmaps.Name, 200);
            
            TiffSaver.Save(location, parsedScreenshotBitmaps.Description);
            TiffSaver.SaveScaled(location, parsedScreenshotBitmaps.Description, 150);
            TiffSaver.SaveScaled(location, parsedScreenshotBitmaps.Description, 200);
            TiffSaver.SaveScaled(location, parsedScreenshotBitmaps.Description, 250);

            foreach (CommodityItemBitmaps item in parsedScreenshotBitmaps.ItemBitmapsList)
            {
                TiffSaver.Save(location, item.Name.Bitmap);
                TiffSaver.Save(location, item.Sell.Bitmap);
                TiffSaver.Save(location, item.Buy.Bitmap);
                TiffSaver.Save(location, item.Supply.Bitmap);
                TiffSaver.Save(location, item.Demand.Bitmap);
                TiffSaver.Save(location, item.GalacticAverage.Bitmap);

                TiffSaver.SaveScaled(location, item.Name.Bitmap, 200);
                TiffSaver.SaveScaled(location, item.Sell.Bitmap, 200);
                TiffSaver.SaveScaled(location, item.Buy.Bitmap, 200);
                TiffSaver.SaveScaled(location, item.Supply.Bitmap, 200);
                TiffSaver.SaveScaled(location, item.Demand.Bitmap, 200);
                TiffSaver.SaveScaled(location, item.GalacticAverage.Bitmap, 200);
            }

            TiffSaver.FlushTiff();
        }
    }
}
