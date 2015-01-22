using System;
using System.Collections.Generic;
using System.Drawing;
using EliteTrader.EliteOcr;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EliteOcrTests
{
    [TestClass]
    [DeploymentItem(@"TestProjectFolder\tessdata", "tessdata")]
    [DeploymentItem(@"TestProjectFolder\x86", "x86")]
    [DeploymentItem(@"TestProjectFolder\x64", "x64")]
    public class TestKarmanVision : TestScreenshotBase
    {
        [TestMethod]
        [DeploymentItem(@"Screenshots\KarmanVision\Screenshot_0005.png", @"Screenshots\KarmanVision")]
        [DeploymentItem(@"Screenshots\KarmanVision\Screenshot_0006.png", @"Screenshots\KarmanVision")]
        [DeploymentItem(@"Screenshots\KarmanVision\Screenshot_0007.png", @"Screenshots\KarmanVision")]
        [DeploymentItem(@"Screenshots\KarmanVision\Screenshot_0008.png", @"Screenshots\KarmanVision")]
        [DeploymentItem(@"Screenshots\KarmanVision\Screenshot_0009.png", @"Screenshots\KarmanVision")]
        [DeploymentItem(@"Screenshots\KarmanVision\combined.txt", @"Screenshots\KarmanVision")]
        public void TestCombined()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            List<Bitmap> bitmaps = new List<Bitmap>();
            bitmaps.Add(new Bitmap(@"Screenshots\KarmanVision\Screenshot_0005.png"));
            bitmaps.Add(new Bitmap(@"Screenshots\KarmanVision\Screenshot_0006.png"));
            bitmaps.Add(new Bitmap(@"Screenshots\KarmanVision\Screenshot_0007.png"));
            bitmaps.Add(new Bitmap(@"Screenshots\KarmanVision\Screenshot_0008.png"));
            bitmaps.Add(new Bitmap(@"Screenshots\KarmanVision\Screenshot_0009.png"));

            ParsedScreenshot actual = screenshotParser.ParseMultiple(bitmaps);

            //Console.WriteLine(actual);
            //string str = Serialize(actual);
            //Console.WriteLine(str);

            Compare(@"Screenshots\KarmanVision\combined.txt", actual);
        }

        //[TestMethod]
        //[DeploymentItem(@"Screenshots\KarmanVision\720p_0000.png", @"Screenshots\KarmanVision")]
        //[DeploymentItem(@"Screenshots\KarmanVision\720p_0001.png", @"Screenshots\KarmanVision")]
        //[DeploymentItem(@"Screenshots\KarmanVision\720p_0002.png", @"Screenshots\KarmanVision")]
        //[DeploymentItem(@"Screenshots\KarmanVision\720p_0003.png", @"Screenshots\KarmanVision")]
        //[DeploymentItem(@"Screenshots\KarmanVision\720p_0004.png", @"Screenshots\KarmanVision")]
        //[DeploymentItem(@"Screenshots\KarmanVision\combined720p.txt", @"Screenshots\KarmanVision")]
        //public void Test720p()
        //{
        //    ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
        //    List<Bitmap> bitmaps = new List<Bitmap>();
        //    bitmaps.Add(new Bitmap(@"Screenshots\KarmanVision\720p_0000.png"));
        //    //bitmaps.Add(new Bitmap(@"Screenshots\KarmanVision\720p_0001.png"));
        //    //bitmaps.Add(new Bitmap(@"Screenshots\KarmanVision\720p_0002.png"));
        //    //bitmaps.Add(new Bitmap(@"Screenshots\KarmanVision\720p_0003.png"));
        //    //bitmaps.Add(new Bitmap(@"Screenshots\KarmanVision\720p_0004.png"));

        //    ParsedScreenshot actual = screenshotParser.ParseMultiple(bitmaps);

        //    Console.WriteLine(actual);
        //    string str = Serialize(actual);
        //    Console.WriteLine(str);

        //    //Compare(@"Screenshots\KarmanVision\combined720p.txt", actual);
        //}
    }
}
