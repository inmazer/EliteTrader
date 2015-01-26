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
    public class TestBaylissLanding : TestScreenshotBase
    {
        [TestMethod]
        [DeploymentItem(@"Screenshots\BaylissLanding\ef404df6-8a16-4c70-a2a2-a71e91e5c8cf_0.png", @"Screenshots\BaylissLanding")]
        [DeploymentItem(@"Screenshots\BaylissLanding\ef404df6-8a16-4c70-a2a2-a71e91e5c8cf_1.png", @"Screenshots\BaylissLanding")]
        [DeploymentItem(@"Screenshots\BaylissLanding\ef404df6-8a16-4c70-a2a2-a71e91e5c8cf_2.png", @"Screenshots\BaylissLanding")]
        [DeploymentItem(@"Screenshots\BaylissLanding\ef404df6-8a16-4c70-a2a2-a71e91e5c8cf_3.png", @"Screenshots\BaylissLanding")]
        [DeploymentItem(@"Screenshots\BaylissLanding\combined.txt", @"Screenshots\SchwarzschildPort")]
        public void TestCombined()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            List<Bitmap> bitmaps = new List<Bitmap>();
            bitmaps.Add(new Bitmap(@"Screenshots\BaylissLanding\ef404df6-8a16-4c70-a2a2-a71e91e5c8cf_0.png"));
            //bitmaps.Add(new Bitmap(@"Screenshots\BaylissLanding\ef404df6-8a16-4c70-a2a2-a71e91e5c8cf_1.png"));
            //bitmaps.Add(new Bitmap(@"Screenshots\BaylissLanding\ef404df6-8a16-4c70-a2a2-a71e91e5c8cf_2.png"));
            //bitmaps.Add(new Bitmap(@"Screenshots\BaylissLanding\ef404df6-8a16-4c70-a2a2-a71e91e5c8cf_3.png"));

            ParsedScreenshot actual = screenshotParser.ParseMultiple(bitmaps);

            Console.WriteLine(actual);
            string str = Serialize(actual);
            Console.WriteLine(str);

            //Compare(@"Screenshots\BaylissLanding\combined.txt", actual);
        }
    }
}
