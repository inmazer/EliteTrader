using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ImageMagick;

namespace EliteTrader.EliteOcr
{
    public static class ImageParser
    {
        public static ParsedScreenshotBitmaps Parse(Bitmap screenshot)
        {
            int screenshotWidth = screenshot.Width;
            int screenshotHeight = screenshot.Height;

            int nameAreaX = (screenshotWidth * 77) / 1920;
            int nameAreaWidth = ((screenshotWidth * 1000) / 1920) - nameAreaX;
            int nameAreaY = (screenshotHeight * 65) / 1080;
            int nameAreaHeight = ((screenshotHeight * 97) / 1080) - nameAreaY;
            Bitmap nameBitmap = GetArea(new Rectangle(nameAreaX, nameAreaY, nameAreaWidth, nameAreaHeight), screenshot, EnumTextSharpeningAlgorithm.Title);

            int infoAreaX = (screenshotWidth * 77) / 1920;
            int infoAreaWidth = ((screenshotWidth * 1300) / 1920) - infoAreaX;
            int infoAreaY = (screenshotHeight * 96) / 1080;
            int infoAreaHeight = ((screenshotHeight * 125) / 1080) - infoAreaY;
            Bitmap descriptionBitmap = GetArea(new Rectangle(infoAreaX, infoAreaY, infoAreaWidth, infoAreaHeight), screenshot, EnumTextSharpeningAlgorithm.SubTitle);

            //int timeAreaX = (screenshotWidth * 1700) / 1920;
            //int timeAreaWidth = ((screenshotWidth * 1839) / 1920) - timeAreaX;
            //int timeAreaY = (screenshotHeight * 65) / 1080;
            //int timeAreaHeight = ((screenshotHeight * 124) / 1080) - timeAreaY;
            //Bitmap timestampBitmap = GetArea(new Rectangle(timeAreaX, timeAreaY, timeAreaWidth, timeAreaHeight), screenshot, EnumTextSharpeningAlgorithm.Time);

            List<CommodityItemBitmaps> itemBitmapsList = GetCommodityItemBitmaps(screenshot);
            
            return new ParsedScreenshotBitmaps(nameBitmap, descriptionBitmap, itemBitmapsList);
        }

        private static List<CommodityItemBitmaps> GetCommodityItemBitmaps(Bitmap screenshot)
        {
            byte[] screenshotBuffer = new byte[screenshot.Width * screenshot.Height * 4];

            BitmapData screenshotData = screenshot.LockBits(new Rectangle(0, 0, screenshot.Width, screenshot.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(screenshotData.Scan0, screenshotBuffer, 0, screenshotBuffer.Length);
            screenshot.UnlockBits(screenshotData);

            //Find row alignment by searching down from 345,350
            int searchStartX = (screenshot.Width * 1270) / 1920;
            int searchStartY = (screenshot.Height * 600) / 1080;

            int sourceRed, sourceGreen, sourceBlue;

            int middleRowStart = -1;
            bool hasSeenBetweenRowPixel = false;
            int rowLength = screenshot.Width * 4;
            int startPixel = (searchStartY * rowLength) + (searchStartX * 4);
            int endPixel = startPixel + (100 * rowLength);

            for (int i = startPixel; i < endPixel; i += rowLength)
            {
                sourceBlue = screenshotBuffer[i];
                sourceGreen = screenshotBuffer[i + 1];
                sourceRed = screenshotBuffer[i + 2];

                bool pixelIsInRow = sourceRed >= 30 && sourceBlue + sourceGreen + sourceRed >= 60;
                if (hasSeenBetweenRowPixel && pixelIsInRow)
                {
                    //This is the first pixel of a row
                    middleRowStart = i / (screenshot.Width * 4);
                    break;
                }
                if (!pixelIsInRow)
                {
                    hasSeenBetweenRowPixel = true;
                }

            }
            if (middleRowStart < 0)
            {
                throw new Exception(string.Format("Unable to find row start"));
            }

            const int totalNumberOfRows = 19; //No matter what the resolution is this seems to be constant (added 1 to catch almost obscured rows)
            const int heightPerItem = 41;
            int maxAllowedItemY = (screenshot.Height * 984) / 1080;
            int itemHeight = (screenshot.Height * heightPerItem) / 1080;
            int numberOfItemsBelow = (maxAllowedItemY - middleRowStart) / itemHeight;
            int numberOfItemsAbove = totalNumberOfRows - numberOfItemsBelow;
            int firstItemY = middleRowStart - (numberOfItemsAbove * itemHeight);

            int commodityNameXOffset = ((screenshot.Width * 83) / 1920) * 4;
            int commodityNameWidth = ((screenshot.Width * 355) / 1920);
            int sellPriceXOffset = ((screenshot.Width * 448) / 1920) * 4;
            int sellPriceWidth = ((screenshot.Width * 81) / 1920);
            int buyPriceXOffset = ((screenshot.Width * 536) / 1920) * 4;
            int buyPriceWidth = ((screenshot.Width * 81) / 1920);
            int demandXOffset = ((screenshot.Width * 715) / 1920) * 4;
            int demandWidth = ((screenshot.Width * 173) / 1920);
            int supplyXOffset = ((screenshot.Width * 900) / 1920) * 4;
            int supplyWidth = ((screenshot.Width * 194) / 1920);
            int galacticAverageXOffset = ((screenshot.Width * 1100) / 1920) * 4;
            int galacticAverageWidth = ((screenshot.Width * 183) / 1920);

            int capturedItemHeight = itemHeight - 10; //Reduce the height of the captured area by 8 pixels to avoid interference from the lines between the items

            byte[] commodityName = new byte[capturedItemHeight * commodityNameWidth * 4];
            byte[] sellPrice = new byte[capturedItemHeight * sellPriceWidth * 4];
            byte[] buyPrice = new byte[capturedItemHeight * buyPriceWidth * 4];
            byte[] demand = new byte[capturedItemHeight * demandWidth * 4];
            byte[] supply = new byte[capturedItemHeight * supplyWidth * 4];
            byte[] galacticAverage = new byte[capturedItemHeight * galacticAverageWidth * 4];

            List<CommodityItemBitmaps> items = new List<CommodityItemBitmaps>();
            for (int i = 0; i < totalNumberOfRows; ++i)
            {
                int thisItemY = firstItemY + (i * itemHeight);

                if (thisItemY < 237)
                {
                    continue; //The item is too high
                }
                if (thisItemY > 950)
                {
                    continue; //The item is too low
                }
                thisItemY += 2; //Offset down to avoid interference from the lines between the items

                bool isHeaderItem = false;
                for (int y = 0; y < capturedItemHeight; ++y)
                {
                    int row = y + thisItemY;

                    if (row < 249)
                    {
                        continue; //This row is too high
                    }
                    if (row > 973)
                    {
                        break; //The rest of the rows are too low
                    }

                    isHeaderItem = CaptureItemPart(y, thisItemY, rowLength, commodityNameWidth,
                        commodityNameXOffset, screenshotBuffer, commodityName, true);

                    if (isHeaderItem)
                    {
                        break;
                    }

                    CaptureItemPart(y, thisItemY, rowLength, sellPriceWidth,
                        sellPriceXOffset, screenshotBuffer, sellPrice, false);
                    CaptureItemPart(y, thisItemY, rowLength, buyPriceWidth,
                        buyPriceXOffset, screenshotBuffer, buyPrice, false);
                    CaptureItemPart(y, thisItemY, rowLength, supplyWidth,
                        supplyXOffset, screenshotBuffer, supply, false);
                    CaptureItemPart(y, thisItemY, rowLength, demandWidth,
                        demandXOffset, screenshotBuffer, demand, false);
                    CaptureItemPart(y, thisItemY, rowLength, galacticAverageWidth,
                        galacticAverageXOffset, screenshotBuffer, galacticAverage, false);
                }

                if (isHeaderItem)
                {
                    continue;
                }
                Bitmap nameBitmapOriginal = CreateBitmap(commodityNameWidth, capturedItemHeight, commodityName);
                Bitmap nameBitmapNoBackground = RemoveBackground(nameBitmapOriginal);
                Bitmap nameBitmap = PostProcess(nameBitmapNoBackground);
                Bitmap[] nameBitmapChars = null;
                //Bitmap[] nameBitmapChars = SplitBitmap(nameBitmap);
                Bitmap sellBitmapOriginal = CreateBitmap(sellPriceWidth, capturedItemHeight, sellPrice);
                Bitmap sellBitmapNoBackground = RemoveBackground(sellBitmapOriginal);
                Bitmap sellBitmap = PostProcess(sellBitmapNoBackground);
                Bitmap[] sellBitmapChars = null;
                //Bitmap[] sellBitmapChars = SplitBitmap(sellBitmap);
                Bitmap buyBitmapOriginal = CreateBitmap(buyPriceWidth, capturedItemHeight, buyPrice);
                Bitmap buyBitmapNoBackground = RemoveBackground(buyBitmapOriginal);
                Bitmap buyBitmap = PostProcess(buyBitmapNoBackground);
                Bitmap[] buyBitmapChars = null;
                //Bitmap[] buyBitmapChars = SplitBitmap(buyBitmap);
                Bitmap supplyBitmapOriginal = CreateBitmap(supplyWidth, capturedItemHeight, supply);
                Bitmap supplyBitmapNoBackground = RemoveBackground(supplyBitmapOriginal);
                Bitmap supplyBitmap = PostProcess(supplyBitmapNoBackground);
                Bitmap[] supplyBitmapChars = null;
                //Bitmap[] supplyBitmapChars = SplitBitmap(supplyBitmap);
                Bitmap demandBitmapOriginal = CreateBitmap(demandWidth, capturedItemHeight, demand);
                Bitmap demandBitmapNoBackground = RemoveBackground(demandBitmapOriginal);
                Bitmap demandBitmap = PostProcess(demandBitmapNoBackground);
                Bitmap[] demandBitmapChars = null;
                //Bitmap[] demandBitmapChars = SplitBitmap(demandBitmap);
                Bitmap galacticAverageBitmapOriginal = CreateBitmap(galacticAverageWidth, capturedItemHeight, galacticAverage);
                Bitmap galacticAverageBitmapNoBackground = RemoveBackground(galacticAverageBitmapOriginal);
                Bitmap galacticAverageBitmap = PostProcess(galacticAverageBitmapNoBackground);
                Bitmap[] galacticAverageBitmapChars = null;
                //Bitmap[] galacticAverageBitmapChars = SplitBitmap(galacticAverageBitmap);

                CommodityItemBitmaps item = new CommodityItemBitmaps(new PartialItemBitmap(nameBitmap, nameBitmapOriginal, nameBitmapNoBackground, nameBitmapChars), new PartialItemBitmap(sellBitmap, sellBitmapOriginal, sellBitmapNoBackground, sellBitmapChars), new PartialItemBitmap(buyBitmap, buyBitmapOriginal, buyBitmapNoBackground, buyBitmapChars), new PartialItemBitmap(supplyBitmap, supplyBitmapOriginal, supplyBitmapNoBackground, supplyBitmapChars),
                    new PartialItemBitmap(demandBitmap, demandBitmapOriginal, demandBitmapNoBackground, demandBitmapChars), new PartialItemBitmap(galacticAverageBitmap, galacticAverageBitmapOriginal, galacticAverageBitmapNoBackground, galacticAverageBitmapChars));
                
                items.Add(item);
            }

            return items;
        }

        private static bool CaptureItemPart(int y, int thisItemY, int rowLength, int itemPartWidth, int itemPartXOffset, byte[] screenshotBuffer, byte[] itemPartBuffer, bool checkForHeaderItem)
        {
            byte sourceBlue, sourceGreen, sourceRed;
            byte targetBlue, targetGreen, targetRed;

            int row = y + thisItemY;
            int firstPixelOnRow = row * rowLength;
            for (int j = 0; j < itemPartWidth * 4; j += 4)
            {
                int screenshotOffset = j + firstPixelOnRow + itemPartXOffset;
                sourceBlue = screenshotBuffer[screenshotOffset];
                sourceGreen = screenshotBuffer[screenshotOffset + 1];
                sourceRed = screenshotBuffer[screenshotOffset + 2];

                targetBlue = sourceBlue;
                targetGreen = sourceGreen;
                targetRed = sourceRed;

                if (checkForHeaderItem && sourceBlue > 90 && sourceGreen > 90 && sourceRed > 90)
                {
                    return true;
                }

                int targetOffset = (y * itemPartWidth * 4) + j;
                itemPartBuffer[targetOffset] = targetBlue;
                itemPartBuffer[targetOffset + 1] = targetGreen;
                itemPartBuffer[targetOffset + 2] = targetRed;
                itemPartBuffer[targetOffset + 3] = 255;
            }
            return false;
        }

        private static Bitmap CreateBitmap(int width, int height, byte[] bytes)
        {
            int numberOfNotBlackPixels = 0;
            for (int i = 0; i < bytes.Length; i += 4)
            {
                if (bytes[i] != 0)
                {
                    ++numberOfNotBlackPixels;
                }
            }

            if (numberOfNotBlackPixels <= 20)
            {
                return null;
            }

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData nameData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(bytes, 0, nameData.Scan0, bytes.Length);
            bitmap.UnlockBits(nameData);

            return bitmap;
        }

        private static Bitmap GetArea(Rectangle area, Bitmap screenshot, EnumTextSharpeningAlgorithm sharpeningAlgorithm)
        {
            Bitmap result = new Bitmap(area.Width, area.Height, PixelFormat.Format32bppArgb);
            BitmapData resultData = result.LockBits(new Rectangle(0, 0, result.Width, result.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            byte[] resultBuffer = new byte[resultData.Stride * resultData.Height];

            BitmapData areaData = screenshot.LockBits(area, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(areaData.Scan0, resultBuffer, 0, resultBuffer.Length);

            screenshot.UnlockBits(areaData);

            switch (sharpeningAlgorithm)
            {
                case EnumTextSharpeningAlgorithm.Title:
                    TitleAlgorithm(resultBuffer);
                    break;
                case EnumTextSharpeningAlgorithm.SubTitle:
                case EnumTextSharpeningAlgorithm.Time:
                    SubTitleAlgorithm(resultBuffer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unknown sharpening algorithm ({0})", sharpeningAlgorithm));
            }

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            result.UnlockBits(resultData);

            result = PostProcess(result);

            return result;
        }

        private static Bitmap RemoveBackground(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }
            using (MagickImage image = new MagickImage(bitmap))
            {
                image.Alpha(AlphaOption.Off);

                bool hasFocus = HasFocus(image);

                using (MagickImage maskForCharacterDetection = image.Separate(Channels.Red).Single())
                {
                    //Create a mask that is black where there is text and white where there is background
                    maskForCharacterDetection.Alpha(AlphaOption.Off);

                    maskForCharacterDetection.ChangeColorSpace(ColorSpace.GRAY);

                    if (hasFocus)
                    {
                        maskForCharacterDetection.Threshold(90);
                    }
                    else
                    {
                        maskForCharacterDetection.Threshold(55);
                        maskForCharacterDetection.Negate();
                    }
                    maskForCharacterDetection.ChangeColorSpace(ColorSpace.GRAY);

                    Dictionary<MagickColor, int> maskHistogram = maskForCharacterDetection.Histogram();
                    int numberOfInterestingPixels;
                    if (!maskHistogram.TryGetValue(new MagickColor(0, 0, 0), out numberOfInterestingPixels))
                    {
                        return null;
                    }
                    if (numberOfInterestingPixels < 22)
                    {
                        return null;
                    }

                    //maskForCharacterDetection.Write(@"c:\tmp\screens\potet4.tif");

                    //Use the mask to detect where the text begins and where the text ends
                    PixelCollection pixels = maskForCharacterDetection.GetReadOnlyPixels();
                    ushort[] values = pixels.GetValues();

                    if (values.Length != pixels.Width*pixels.Height)
                    {
                        throw new Exception(
                            string.Format(
                                "Unexpected pixel array length ({0}). Image has width ({1}) and height ({2})",
                                values.Length, pixels.Width, pixels.Height));
                    }

                    int width = pixels.Width;
                    int leftmostCharacterStart = pixels.Width - 1;
                    int rightmostCharacterEnd = 0;
                    int highestCharacterPixel = pixels.Height - 1;
                    int lowestCharacterPixel = 0;
                    for (int i = 0; i < values.Length; ++i)
                    {
                        int xForComparison = i%width;
                        int yForComparison = i/width;
                        //if (xForComparison > leftmostCharacterStart && xForComparison < rightmostCharacterEnd)
                        //{
                        //    i += (rightmostCharacterEnd - xForComparison);
                        //    xForComparison = i % width;
                        //}
                        ushort value = values[i];
                        if (value == 0)
                        {
                            if (xForComparison < leftmostCharacterStart)
                            {
                                leftmostCharacterStart = xForComparison;
                            }
                            if (xForComparison > rightmostCharacterEnd)
                            {
                                rightmostCharacterEnd = xForComparison;
                            }
                            if (yForComparison < highestCharacterPixel)
                            {
                                highestCharacterPixel = yForComparison;
                            }
                            if (yForComparison > lowestCharacterPixel)
                            {
                                lowestCharacterPixel = yForComparison;
                            }
                        }
                    }
                    leftmostCharacterStart -= 3;
                    if (leftmostCharacterStart < 0)
                    {
                        leftmostCharacterStart = 0;
                    }
                    rightmostCharacterEnd += 3;
                    if (rightmostCharacterEnd >= pixels.Width)
                    {
                        rightmostCharacterEnd = pixels.Width - 1;
                    }
                    highestCharacterPixel -= 2;
                    if (highestCharacterPixel < 0)
                    {
                        highestCharacterPixel = 0;
                    }
                    lowestCharacterPixel += 2;
                    if (lowestCharacterPixel >= pixels.Height)
                    {
                        lowestCharacterPixel = pixels.Height - 1;
                    }

                    //Remove the parts of the image that does not have text
                    int newWidth = rightmostCharacterEnd - leftmostCharacterStart;
                    int newHeight = lowestCharacterPixel - highestCharacterPixel;
                    image.Crop(
                        new MagickGeometry(new Rectangle(leftmostCharacterStart, highestCharacterPixel, newWidth,
                            newHeight)));
                    maskForCharacterDetection.Crop(
                        new MagickGeometry(new Rectangle(leftmostCharacterStart, highestCharacterPixel, newWidth,
                            newHeight)));

                    if (!hasFocus)
                    {
                        //Create an image where the text is removed. Used for getting the average value of the background
                        using (MagickImage noText = image.Clone())
                        {
                            noText.Composite(maskForCharacterDetection, new MagickGeometry(image.Width, image.Height),
                                CompositeOperator.Multiply);

                            int average = GetAverageExceptBlack(noText);

                            using (MagickImage textMask = image.Clone())
                            {
                                textMask.Alpha(AlphaOption.Off);

                                textMask.ChangeColorSpace(ColorSpace.GRAY);
                                if (average > 25000)
                                {
                                    textMask.Threshold(65);
                                }
                                else if (average > 20000)
                                {
                                    textMask.Threshold(60);
                                }
                                else if (average > 15000)
                                {
                                    textMask.Threshold(55);
                                }
                                else
                                {
                                    textMask.Threshold(50);
                                }
                                textMask.ChangeColorSpace(ColorSpace.GRAY);

                                image.ChangeColorSpace(ColorSpace.GRAY);

                                image.Composite(textMask, new MagickGeometry(image.Width, image.Height),
                                    CompositeOperator.Multiply);
                            }
                        }
                    }
                    else
                    {
                        maskForCharacterDetection.Negate();
                        maskForCharacterDetection.ChangeColorSpace(ColorSpace.GRAY);

                        image.ChangeColorSpace(ColorSpace.GRAY);
                        image.Negate();

                        image.Modulate(175, 100, 100);

                        image.Composite(maskForCharacterDetection, new MagickGeometry(image.Width, image.Height),
                            CompositeOperator.Multiply);
                    }
                    return GetBitmapFromImage(image);
                }
            }
        }

        public static Bitmap GetBitmapFromImage(MagickImage image)
        {
            image.CompressionMethod = CompressionMethod.NoCompression;
            image.Format = MagickFormat.Tif;

            MemoryStream ms = new MemoryStream((image.Width * image.Height * 3) + 4096); //Create a buffer large enough to contain the 24bpp uncompressed Tiff
            image.Write(ms);
            ms.Position = 0;
            Bitmap result = new Bitmap(ms);
            return result;
        }

        private static bool HasFocus(MagickImage image)
        {
            int numberOfPixelsWithHighRed = 0;
            Dictionary<MagickColor, int> histogram = image.Histogram();
            foreach (KeyValuePair<MagickColor, int> pair in histogram)
            {
                MagickColor color = pair.Key;
                int numberOfPixels = pair.Value;

                if (color.R > 60000)
                {
                    numberOfPixelsWithHighRed += numberOfPixels;
                }
            }
            double percentageHighRed = (double)numberOfPixelsWithHighRed / (image.Width * image.Height);

            return percentageHighRed > 0.50;
        }

        private static Bitmap PostProcess(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }
            using (MagickImage image = new MagickImage(bitmap))
            {
                image.Alpha(AlphaOption.Off);

                image.Scale(new Percentage(500));

                image.Blur(0, 2);
                image.Unsharpmask(0, 5);

                using (MagickImage clone = image.Clone())
                {
                    clone.Alpha(AlphaOption.Off);
                    clone.ChangeColorSpace(ColorSpace.GRAY);

                    int average = GetAverageExceptBlack(clone);
                    int averagePercentage = (int) (((double) average/65535)*100);

                    clone.Threshold(new Percentage(averagePercentage + 6));

                    image.Composite(clone, new MagickGeometry(clone.Width, clone.Height), CompositeOperator.Multiply);
                    image.Alpha(AlphaOption.Off);

                    image.ChangeColorSpace(ColorSpace.GRAY);

                    return GetBitmapFromImage(image);
                }
            }
        }

        private static int GetAverageExceptBlack(MagickImage image)
        {
            image.Alpha(AlphaOption.Off);
            image.ChangeColorSpace(ColorSpace.GRAY);

            PixelCollection pixels = image.GetReadOnlyPixels();
            ushort[] values = pixels.GetValues();
            int numberOfPixels = 0;
            long total = 0;
            for (int i = 0; i < values.Length; ++i)
            {
                ushort value = values[i];
                if (value != 0)
                {
                    total += value;
                    ++numberOfPixels;
                }
            }

            int average = 0;

            if (numberOfPixels != 0)
            {
                average = (int)(total / numberOfPixels);
            }

            return average;
        }

        private static void SubTitleAlgorithm(byte[] resultBuffer)
        {
            byte sourceRed, sourceGreen, sourceBlue;
            byte resultRed = 0, resultGreen = 0, resultBlue = 0;

            for (int i = 0; i < resultBuffer.Length; i += 4)
            {
                sourceBlue = resultBuffer[i];
                sourceGreen = resultBuffer[i + 1];
                sourceRed = resultBuffer[i + 2];

                if (sourceRed < 90)
                {
                    resultRed = 0;
                    resultGreen = 0;
                    resultBlue = 0;
                }
                else
                {
                    resultRed = sourceRed;
                    resultGreen = sourceGreen;
                    resultBlue = sourceBlue;
                }

                resultBuffer[i] = resultBlue;
                resultBuffer[i + 1] = resultGreen;
                resultBuffer[i + 2] = resultRed;
                resultBuffer[i + 3] = 255;
            }
        }

        private static void TitleAlgorithm(byte[] resultBuffer)
        {
            byte sourceRed, sourceGreen, sourceBlue;
            byte resultRed, resultGreen, resultBlue;
            for (int i = 0; i < resultBuffer.Length; i += 4)
            {
                sourceBlue = resultBuffer[i];
                sourceGreen = resultBuffer[i + 1];
                sourceRed = resultBuffer[i + 2];

                if (sourceBlue > 100 && sourceGreen > 100 && sourceRed > 100)
                {
                    resultRed = sourceRed;
                    resultGreen = sourceGreen;
                    resultBlue = sourceBlue;
                }
                else
                {
                    resultRed = 0;
                    resultGreen = 0;
                    resultBlue = 0;
                }

                resultBuffer[i] = resultBlue;
                resultBuffer[i + 1] = resultGreen;
                resultBuffer[i + 2] = resultRed;
                resultBuffer[i + 3] = 255;
            }
        }

        private enum EnumTextSharpeningAlgorithm
        {
            Title,
            SubTitle,
            Time
        }

        private static Bitmap[] SplitBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] bytes = new byte[bitmapData.Width * bitmapData.Height * 4];
            Marshal.Copy(bitmapData.Scan0, bytes, 0, bytes.Length);
            bitmap.UnlockBits(bitmapData);

            bool[] characterDetectionArray = new bool[bitmap.Width];
            for (int x = 0; x < (bitmap.Width * 4); x += 4)
            {
                for (int y = 0; y < bytes.Length; y += (bitmap.Width * 4))
                {
                    int position = x + y;
                    byte value = bytes[position];
                    if (value != 0)
                    {
                        characterDetectionArray[x / 4] = true;
                    }
                }
            }

            List<int> characterStartXList = new List<int>();
            List<int> characterEndXList = new List<int>();

            bool isInsideChar = false;

            for (int i = 0; i < characterDetectionArray.Length; ++i)
            {
                if (characterDetectionArray[i])
                {
                    if (!isInsideChar)
                    {
                        characterStartXList.Add(i);
                        isInsideChar = true;
                    }
                }
                else
                {
                    if (isInsideChar)
                    {
                        characterEndXList.Add(i - 1);
                        isInsideChar = false;
                    }
                }
            }

            if (characterStartXList.Count != characterEndXList.Count)
            {
                throw new Exception(string.Format("CharacterStartX and CharacterEndX did not have the same length, unable to split characters correctly"));
            }

            List<Bitmap> characterBitmaps = new List<Bitmap>();
            for (int i = 0; i < characterStartXList.Count; ++i)
            {
                int startX = characterStartXList[i];
                if (startX == 0)
                {
                    throw new Exception(string.Format("Cannot have a character that starts on the first column"));
                }

                int endX = characterEndXList[i];
                if (endX == bitmap.Width - 1)
                {
                    throw new Exception(string.Format("Cannot have a character that ends on the last column"));
                }

                if (endX <= startX)
                {
                    throw new Exception(string.Format("Invalid character start x ({0}) and end x ({1})", startX, endX));
                }

                int width = (endX - startX) + 3;

                bitmapData = bitmap.LockBits(new Rectangle(startX - 1, 0, width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                byte[] characterBytes = new byte[width * bitmapData.Height * 4];
                Marshal.Copy(bitmapData.Scan0, characterBytes, 0, characterBytes.Length);
                bitmap.UnlockBits(bitmapData);

                int numberOfPixels = 0;
                for (int j = 0; j < characterBytes.Length; j += 4)
                {
                    if (characterBytes[j] != 0)
                    {
                        ++numberOfPixels;
                    }
                }
                if (numberOfPixels < 300)
                {
                    continue;
                }

                Bitmap characterBitmap = new Bitmap(width, bitmap.Height);
                BitmapData characterBitmapData = characterBitmap.LockBits(new Rectangle(0, 0, width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                Marshal.Copy(characterBytes, 0, characterBitmapData.Scan0, characterBytes.Length);
                characterBitmap.UnlockBits(characterBitmapData);

                characterBitmaps.Add(characterBitmap);
            }

            return characterBitmaps.ToArray();
        }
    }
}
