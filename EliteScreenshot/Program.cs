using System;
using System.Drawing;
using EliteTrader.EliteOcr;

namespace EliteTrader.EliteScreenshot
{
    public static class Program
    {
        public static void Main()
        {
            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);

            //TessHelper.UpdateDictionary(@"C:\Git\EliteTrader\tessdata\edl.user-words", new CommodityNameRepository());
            //return;
            
            Bitmap screenshot = new Bitmap(@"c:\tmp\screens\1.bmp");

            ParsedScreenshot parsedScreenshot = screenshotParser.Parse(screenshot);
            

            Console.WriteLine(parsedScreenshot);
        }
    }
}
