using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks.Dataflow;
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
            Bitmap bitmap;
            using (Bitmap bitmapTmp = new Bitmap(filename))
            {
                bitmap = new Bitmap(bitmapTmp);
            }
            return Parse(bitmap);
        }

        public ParsedScreenshot Parse(Bitmap screenshot)
        {
            using (ParsedScreenshotBitmaps parsedScreenshotBitmaps = ImageParser.Parse(screenshot))
            {
                OcrParser ocrParser = new OcrParser(_commodityNameMatcher);

                return ocrParser.Parse(parsedScreenshotBitmaps);
            }
        }

        public ParsedScreenshot ParseMultiple(List<string> filenames)
        {
            List<Bitmap> screenshots = new List<Bitmap>();
            foreach (string filename in filenames)
            {
                Bitmap bitmap;
                using (Bitmap bitmapTmp = new Bitmap(filename))
                {
                    bitmap = new Bitmap(bitmapTmp);
                }
                screenshots.Add(bitmap);
            }

            return ParseMultiple(screenshots);
        }

        public ParsedScreenshot ParseMultiple(List<Bitmap> screenshots)
        {
            if (screenshots.Count == 0)
            {
                throw new Exception(string.Format("Provide at least one screenshot"));
            }
            if (screenshots.Count == 1)
            {
                return Parse(screenshots[0]);
            }

            ExecutionDataflowBlockOptions processorOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Math.Max(Environment.ProcessorCount - 1, 1)
            };
            BufferBlock<Bitmap> inputBufferBlock = new BufferBlock<Bitmap>();
            TransformBlock<Bitmap, ParsedScreenshot> processor = new TransformBlock<Bitmap, ParsedScreenshot>(a => Parse(a), processorOptions);
            BufferBlock<ParsedScreenshot> outputBufferBlock = new BufferBlock<ParsedScreenshot>();

            using (inputBufferBlock.LinkTo(processor, new DataflowLinkOptions { PropagateCompletion = true }))
            using (processor.LinkTo(outputBufferBlock, new DataflowLinkOptions {PropagateCompletion = true}))
            {
                foreach (Bitmap screenshot in screenshots)
                {
                    inputBufferBlock.Post(screenshot);
                }

                inputBufferBlock.Complete();
                processor.Completion.Wait();

                IList<ParsedScreenshot> parsedScreenshots;
                if (outputBufferBlock.TryReceiveAll(out parsedScreenshots))
                {
                    ParsedScreenshot first = parsedScreenshots[0];
                    for (int i = 1; i < parsedScreenshots.Count; ++i)
                    {
                        ParsedScreenshot other = parsedScreenshots[i];
                        first.Add(other);
                    }

                    return first;
                }
            }
            
            throw new Exception(string.Format("Got no results back after sending in ({0}) for processing", screenshots.Count));
        }

        public string ParseClock(string path)
        {
            return ParseClock(new Bitmap(path));
        }

        public string ParseClock(Bitmap screenshot)
        {
            using (Bitmap bitmap = ImageParser.ParseClock(screenshot))
            {
                OcrParser ocrParser = new OcrParser(_commodityNameMatcher);

                return ocrParser.ParseClock(bitmap);
            }
        }

        public bool IsCommoditiesScreen(string path)
        {
            return IsCommoditiesScreen(new Bitmap(path));
        }

        public bool IsCommoditiesScreen(Bitmap bitmap)
        {
            try
            {
                string clockStr = ParseClock(bitmap);

                return OcrParser.IsCommoditiesScreen(clockStr);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
