using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EliteTrader.EliteOcr;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EliteOcrTests
{
    [TestClass]
    [DeploymentItem(@"TestProjectFolder\tessdata", "tessdata")]
    [DeploymentItem(@"TestProjectFolder\x86", "x86")]
    [DeploymentItem(@"TestProjectFolder\x64", "x64")]
    public class TestWeynDock : TestScreenshotBase
    {
        [TestMethod]
        [DeploymentItem(@"Screenshots\WeynDock\ab81d0f6-f07b-44db-a1f2-7901bea20873_0.png", @"Screenshots\WeynDock")]
        [DeploymentItem(@"Screenshots\WeynDock\ab81d0f6-f07b-44db-a1f2-7901bea20873_1.png", @"Screenshots\WeynDock")]
        [DeploymentItem(@"Screenshots\WeynDock\ab81d0f6-f07b-44db-a1f2-7901bea20873_2.png", @"Screenshots\WeynDock")]
        [DeploymentItem(@"Screenshots\WeynDock\ab81d0f6-f07b-44db-a1f2-7901bea20873_3.png", @"Screenshots\WeynDock")]
        [DeploymentItem(@"Screenshots\WeynDock\combined.txt", @"Screenshots\WeynDock")]
        public void TestCombined()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            List<Bitmap> bitmaps = new List<Bitmap>();
            bitmaps.Add(new Bitmap(@"Screenshots\WeynDock\ab81d0f6-f07b-44db-a1f2-7901bea20873_0.png"));
            bitmaps.Add(new Bitmap(@"Screenshots\WeynDock\ab81d0f6-f07b-44db-a1f2-7901bea20873_1.png"));
            bitmaps.Add(new Bitmap(@"Screenshots\WeynDock\ab81d0f6-f07b-44db-a1f2-7901bea20873_2.png"));
            bitmaps.Add(new Bitmap(@"Screenshots\WeynDock\ab81d0f6-f07b-44db-a1f2-7901bea20873_3.png"));

            ParsedScreenshot actual = screenshotParser.ParseMultiple(bitmaps);

            //Console.WriteLine(actual);
            //string str = Serialize(actual);
            //Console.WriteLine(str);

            Compare(@"Screenshots\WeynDock\combined.txt", actual);
        }
    }
}
