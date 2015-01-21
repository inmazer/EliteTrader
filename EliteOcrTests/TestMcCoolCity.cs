using System;
using EliteTrader.EliteOcr;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EliteOcrTests
{
    [TestClass]
    [DeploymentItem(@"TestProjectFolder\tessdata", "tessdata")]
    [DeploymentItem(@"TestProjectFolder\x86", "x86")]
    [DeploymentItem(@"TestProjectFolder\x64", "x64")]
    public class TestMcCoolCity : TestScreenshotBase
    {
        [TestMethod]
        [DeploymentItem(@"Screenshots\McCoolCity\Screenshot_0000.png", @"Screenshots\McCoolCity")]
        [DeploymentItem(@"Screenshots\McCoolCity\0.txt", @"Screenshots\McCoolCity")]
        public void Test0()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            ParsedScreenshot actual = screenshotParser.Parse(@"Screenshots\McCoolCity\Screenshot_0000.png");

            //Console.WriteLine(actual);
            //string str = Serialize(actual);
            //Console.WriteLine(str);

            Compare(@"Screenshots\McCoolCity\0.txt", actual);
        }

        [TestMethod]
        [DeploymentItem(@"Screenshots\McCoolCity\Screenshot_0001.png", @"Screenshots\McCoolCity")]
        [DeploymentItem(@"Screenshots\McCoolCity\1.txt", @"Screenshots\McCoolCity")]
        public void Test1()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            ParsedScreenshot actual = screenshotParser.Parse(@"Screenshots\McCoolCity\Screenshot_0001.png");

            //Console.WriteLine(actual);
            //string str = Serialize(actual);
            //Console.WriteLine(str);

            Compare(@"Screenshots\McCoolCity\1.txt", actual);
        }

        [TestMethod]
        [DeploymentItem(@"Screenshots\McCoolCity\Screenshot_0002.png", @"Screenshots\McCoolCity")]
        [DeploymentItem(@"Screenshots\McCoolCity\2.txt", @"Screenshots\McCoolCity")]
        public void Test2()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);
            ParsedScreenshot actual = screenshotParser.Parse(@"Screenshots\McCoolCity\Screenshot_0002.png");

            //Console.WriteLine(actual);
            //string str = Serialize(actual);
            //Console.WriteLine(str);

            Compare(@"Screenshots\McCoolCity\2.txt", actual);
        }
    }
}
