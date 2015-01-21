using System;
using EliteTrader.EliteOcr;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EliteOcrTests
{
    [TestClass]
    [DeploymentItem(@"TestProjectFolder\tessdata", "tessdata")]
    [DeploymentItem(@"TestProjectFolder\x86", "x86")]
    [DeploymentItem(@"TestProjectFolder\x64", "x64")]
    public class TestMozhayskyGateway : TestScreenshotBase
    {
        [TestMethod]
        [DeploymentItem(@"Screenshots\MozhayskyGateway\Screenshot_0000.png", @"Screenshots\MozhayskyGateway")]
        [DeploymentItem(@"Screenshots\MozhayskyGateway\0.txt", @"Screenshots\MozhayskyGateway")]
        public void Test0()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            ParsedScreenshot actual = screenshotParser.Parse(@"Screenshots\MozhayskyGateway\Screenshot_0000.png");

            //Console.WriteLine(actual);
            //string str = Serialize(actual);
            //Console.WriteLine(str);

            Compare(@"Screenshots\MozhayskyGateway\0.txt", actual);
        }

        [TestMethod]
        [DeploymentItem(@"Screenshots\MozhayskyGateway\Screenshot_0001.png", @"Screenshots\MozhayskyGateway")]
        [DeploymentItem(@"Screenshots\MozhayskyGateway\1.txt", @"Screenshots\MozhayskyGateway")]
        public void Test1()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            ParsedScreenshot actual = screenshotParser.Parse(@"Screenshots\MozhayskyGateway\Screenshot_0001.png");

            //Console.WriteLine(actual);
            //string str = Serialize(actual);
            //Console.WriteLine(str);

            Compare(@"Screenshots\MozhayskyGateway\1.txt", actual);
        }

        [TestMethod]
        [DeploymentItem(@"Screenshots\MozhayskyGateway\Screenshot_0002.png", @"Screenshots\MozhayskyGateway")]
        [DeploymentItem(@"Screenshots\MozhayskyGateway\2.txt", @"Screenshots\MozhayskyGateway")]
        public void Test2()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            ParsedScreenshot actual = screenshotParser.Parse(@"Screenshots\MozhayskyGateway\Screenshot_0002.png");

            //Console.WriteLine(actual);
            //string str = Serialize(actual);
            //Console.WriteLine(str);

            Compare(@"Screenshots\MozhayskyGateway\2.txt", actual);
        }

        [TestMethod]
        [DeploymentItem(@"Screenshots\MozhayskyGateway\Screenshot_0003.png", @"Screenshots\MozhayskyGateway")]
        [DeploymentItem(@"Screenshots\MozhayskyGateway\3.txt", @"Screenshots\MozhayskyGateway")]
        public void Test3()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            ParsedScreenshot actual = screenshotParser.Parse(@"Screenshots\MozhayskyGateway\Screenshot_0003.png");

            //Console.WriteLine(actual);
            //string str = Serialize(actual);
            //Console.WriteLine(str);

            Compare(@"Screenshots\MozhayskyGateway\3.txt", actual);
        }
    }
}
