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
    public class TestSchwarzschildPort : TestScreenshotBase
    {
        [TestMethod]
        [DeploymentItem(@"Screenshots\SchwarzschildPort\Screenshot_0000.png", @"Screenshots\SchwarzschildPort")]
        [DeploymentItem(@"Screenshots\SchwarzschildPort\Screenshot_0001.png", @"Screenshots\SchwarzschildPort")]
        [DeploymentItem(@"Screenshots\SchwarzschildPort\Screenshot_0002.png", @"Screenshots\SchwarzschildPort")]
        [DeploymentItem(@"Screenshots\SchwarzschildPort\combined.txt", @"Screenshots\SchwarzschildPort")]
        public void TestCombined()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            List<Bitmap> bitmaps = new List<Bitmap>();
            bitmaps.Add(new Bitmap(@"Screenshots\SchwarzschildPort\Screenshot_0000.png"));
            bitmaps.Add(new Bitmap(@"Screenshots\SchwarzschildPort\Screenshot_0001.png"));
            bitmaps.Add(new Bitmap(@"Screenshots\SchwarzschildPort\Screenshot_0002.png"));

            ParsedScreenshot actual = screenshotParser.ParseMultiple(bitmaps);

            //Console.WriteLine(actual);
            //string str = Serialize(actual);
            //Console.WriteLine(str);

            Compare(@"Screenshots\SchwarzschildPort\combined.txt", actual);
        }

        [TestMethod]
        [DeploymentItem(@"Screenshots\SchwarzschildPort\Screenshot_0003.png", @"Screenshots\SchwarzschildPort")]
        [DeploymentItem(@"Screenshots\SchwarzschildPort\Screenshot_0004.png", @"Screenshots\SchwarzschildPort")]
        [DeploymentItem(@"Screenshots\SchwarzschildPort\Screenshot_0005.png", @"Screenshots\SchwarzschildPort")]
        [DeploymentItem(@"Screenshots\SchwarzschildPort\combined2.txt", @"Screenshots\SchwarzschildPort")]
        public void TestCombined2()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            List<Bitmap> bitmaps = new List<Bitmap>();
            bitmaps.Add(new Bitmap(@"Screenshots\SchwarzschildPort\Screenshot_0003.png"));
            bitmaps.Add(new Bitmap(@"Screenshots\SchwarzschildPort\Screenshot_0004.png"));
            bitmaps.Add(new Bitmap(@"Screenshots\SchwarzschildPort\Screenshot_0005.png"));

            ParsedScreenshot actual = screenshotParser.ParseMultiple(bitmaps);

            //Console.WriteLine(actual);
            //string str = Serialize(actual);
            //Console.WriteLine(str);

            Compare(@"Screenshots\SchwarzschildPort\combined2.txt", actual);
        }
    }
}
