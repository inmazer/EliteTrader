using System;
using System.Drawing;
using EliteTrader.EliteOcr.Data;
using EliteTrader.EliteOcr.Interfaces;
using EliteTrader.EliteOcr.Tesseract;

namespace EliteTrader.EliteOcr
{
    public class ScreenshotParser
    {
        private readonly ICommodityNameMatcher _commodityNameMatcher;

        public ScreenshotParser(string folderAboveTessData)
            : this(folderAboveTessData, new CommodityItemNameMatcher())
        {
        }

        public ScreenshotParser(string folderAboveTessData, ICommodityNameMatcher commodityNameMatcher)
        {
            _commodityNameMatcher = commodityNameMatcher;

            Environment.SetEnvironmentVariable("TESSDATA_PREFIX", folderAboveTessData);
        }

        public ParsedScreenshot Parse(string filename)
        {
            return Parse(new Bitmap(filename));
        }

        public ParsedScreenshot Parse(Bitmap screenshot)
        {
            using (ParsedScreenshotBitmaps parsedScreenshotBitmaps = ImageParser.Parse(screenshot))
            {
                OcrParser ocrParser = new OcrParser(_commodityNameMatcher);

                return ocrParser.Parse(parsedScreenshotBitmaps);
            }
        }
    }
}
