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
    public class TestFortKlarix : TestScreenshotBase
    {
        [TestMethod]
        [DeploymentItem(@"Screenshots\FortKlarix\0.png", @"Screenshots\FortKlarix")]
        [DeploymentItem(@"Screenshots\FortKlarix\0.txt", @"Screenshots\FortKlarix")]
        public void Test0()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            ParsedScreenshot actual = screenshotParser.Parse(@"Screenshots\FortKlarix\0.png");

            //Console.WriteLine(actual);
            //string str = Serialize(actual);
            //Console.WriteLine(str);

            Compare(@"Screenshots\FortKlarix\0.txt", actual); 
        }

        [TestMethod]
        [DeploymentItem(@"Screenshots\FortKlarix\1.png", @"Screenshots\FortKlarix")]
        [DeploymentItem(@"Screenshots\FortKlarix\1.txt", @"Screenshots\FortKlarix")]
        public void Test1()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            ParsedScreenshot actual = screenshotParser.Parse(@"Screenshots\FortKlarix\1.png");

            //Console.WriteLine(actual);
            //string str = Serialize(actual);
            //Console.WriteLine(str);

            Compare(@"Screenshots\FortKlarix\1.txt", actual);
        }

        [TestMethod]
        [DeploymentItem(@"Screenshots\FortKlarix\2.png", @"Screenshots\FortKlarix")]
        [DeploymentItem(@"Screenshots\FortKlarix\2.txt", @"Screenshots\FortKlarix")]
        public void Test2()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            ParsedScreenshot actual = screenshotParser.Parse(@"Screenshots\FortKlarix\2.png");

            //Console.WriteLine(actual);
            //string str = Serialize(actual);
            //Console.WriteLine(str);

            Compare(@"Screenshots\FortKlarix\2.txt", actual);
        }

        [TestMethod]
        [DeploymentItem(@"Screenshots\FortKlarix\3.png", @"Screenshots\FortKlarix")]
        [DeploymentItem(@"Screenshots\FortKlarix\3.txt", @"Screenshots\FortKlarix")]
        public void Test3()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            ParsedScreenshot actual = screenshotParser.Parse(@"Screenshots\FortKlarix\3.png");

            //Console.WriteLine(actual);
            //string str = Serialize(actual);
            //Console.WriteLine(str);

            Compare(@"Screenshots\FortKlarix\3.txt", actual);
        }

        [TestMethod]
        [DeploymentItem(@"Screenshots\FortKlarix\4.png", @"Screenshots\FortKlarix")]
        [DeploymentItem(@"Screenshots\FortKlarix\4.txt", @"Screenshots\FortKlarix")]
        public void Test4()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            ParsedScreenshot actual = screenshotParser.Parse(@"Screenshots\FortKlarix\4.png");

            //Console.WriteLine(actual);
            //string str = Serialize(actual);
            //Console.WriteLine(str);

            Compare(@"Screenshots\FortKlarix\4.txt", actual);
        }

        [TestMethod]
        [DeploymentItem(@"Screenshots\FortKlarix\5.png", @"Screenshots\FortKlarix")]
        [DeploymentItem(@"Screenshots\FortKlarix\5.txt", @"Screenshots\FortKlarix")]
        public void Test5()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            ParsedScreenshot actual = screenshotParser.Parse(@"Screenshots\FortKlarix\5.png");

            //Console.WriteLine(actual);
            //string str = Serialize(actual);
            //Console.WriteLine(str);

            Compare(@"Screenshots\FortKlarix\5.txt", actual);
        }

        [TestMethod]
        [DeploymentItem(@"Screenshots\FortKlarix\0.png", @"Screenshots\FortKlarix")]
        [DeploymentItem(@"Screenshots\FortKlarix\1.png", @"Screenshots\FortKlarix")]
        [DeploymentItem(@"Screenshots\FortKlarix\2.png", @"Screenshots\FortKlarix")]
        [DeploymentItem(@"Screenshots\FortKlarix\3.png", @"Screenshots\FortKlarix")]
        [DeploymentItem(@"Screenshots\FortKlarix\4.png", @"Screenshots\FortKlarix")]
        [DeploymentItem(@"Screenshots\FortKlarix\5.png", @"Screenshots\FortKlarix")]
        [DeploymentItem(@"Screenshots\FortKlarix\combined.txt", @"Screenshots\FortKlarix")]
        public void TestCombined()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            List<Bitmap> bitmaps = new List<Bitmap>();
            bitmaps.Add(new Bitmap(@"Screenshots\FortKlarix\0.png"));
            bitmaps.Add(new Bitmap(@"Screenshots\FortKlarix\1.png"));
            bitmaps.Add(new Bitmap(@"Screenshots\FortKlarix\2.png"));
            bitmaps.Add(new Bitmap(@"Screenshots\FortKlarix\3.png"));
            bitmaps.Add(new Bitmap(@"Screenshots\FortKlarix\4.png"));
            bitmaps.Add(new Bitmap(@"Screenshots\FortKlarix\5.png"));

            ParsedScreenshot actual = screenshotParser.ParseMultiple(bitmaps);

            //Console.WriteLine(actual);
            //string str = Serialize(actual);
            //Console.WriteLine(str);

            Compare(@"Screenshots\FortKlarix\combined.txt", actual);
        }
    }
}
