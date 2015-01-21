using System;
using EliteTrader.EliteOcr;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EliteOcrTests
{
    [TestClass]
    [DeploymentItem(@"TestProjectFolder\tessdata", "tessdata")]
    [DeploymentItem(@"TestProjectFolder\x86", "x86")]
    [DeploymentItem(@"TestProjectFolder\x64", "x64")]
    public class TestClock : TestScreenshotBase
    {
        [TestMethod]
        [DeploymentItem(@"Screenshots\Clock\Screenshot_0000.png", @"Screenshots\Clock")]
        [DeploymentItem(@"Screenshots\Clock\Screenshot_0001.png", @"Screenshots\Clock")]
        [DeploymentItem(@"Screenshots\Clock\Screenshot_0002.png", @"Screenshots\Clock")]
        [DeploymentItem(@"Screenshots\Clock\Screenshot_0003.png", @"Screenshots\Clock")]
        [DeploymentItem(@"Screenshots\Clock\Screenshot_0004.png", @"Screenshots\Clock")]
        [DeploymentItem(@"Screenshots\Clock\Screenshot_0005.png", @"Screenshots\Clock")]
        [DeploymentItem(@"Screenshots\Clock\Screenshot_0006.png", @"Screenshots\Clock")]
        [DeploymentItem(@"Screenshots\Clock\Screenshot_0007.png", @"Screenshots\Clock")]
        [DeploymentItem(@"Screenshots\Clock\Screenshot_0008.png", @"Screenshots\Clock")]
        public void TestClockParsing()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);

            Assert.AreEqual("17:27:30", screenshotParser.ParseClock(@"Screenshots\Clock\Screenshot_0000.png"));
            Assert.AreEqual("17:27:31", screenshotParser.ParseClock(@"Screenshots\Clock\Screenshot_0001.png"));
            Assert.AreEqual("17:27:33", screenshotParser.ParseClock(@"Screenshots\Clock\Screenshot_0002.png"));
            Assert.AreEqual("17:27:34", screenshotParser.ParseClock(@"Screenshots\Clock\Screenshot_0003.png"));
            Assert.AreEqual("17:27:35", screenshotParser.ParseClock(@"Screenshots\Clock\Screenshot_0004.png"));
            Assert.AreEqual("17:27:36", screenshotParser.ParseClock(@"Screenshots\Clock\Screenshot_0005.png"));
            Assert.AreEqual("17:27:37", screenshotParser.ParseClock(@"Screenshots\Clock\Screenshot_0006.png"));
            Assert.AreEqual("17:27:38", screenshotParser.ParseClock(@"Screenshots\Clock\Screenshot_0007.png"));
            Assert.AreEqual("17:27:39", screenshotParser.ParseClock(@"Screenshots\Clock\Screenshot_0008.png"));
        }
    }
}
