using System;
using System.Drawing;

namespace EliteTrader.EliteOcr
{
    public static class ImageCoordinatesRepository
    {
        private readonly static AspectRatio _a16_9 = new AspectRatio(16, 9);
        private readonly static AspectRatio _a16_10 = new AspectRatio(16, 10);
        private readonly static AspectRatio _a4_3 = new AspectRatio(4, 3);

        public static ImageCoordinates GetImageCoordinates(int width, int height)
        {
            if (width < 1920 || height < 1080)
            {
                throw new Exception(string.Format("The only supported resolutions are 1920x1080 and above at the moment. Please send in your screenshot and we can take a look at adding this resolution"));
            }

            AspectRatio aspectRatio = new AspectRatio(width, height);

            if (aspectRatio == _a16_9)
            {
                const int originalImageWidth = 1920;
                const int originalImageHeight = 1080;

                int clockAreaX = (width * 1705) / originalImageWidth;
                int clockAreaWidth = ((width * 1839) / originalImageWidth) - clockAreaX;
                int clockAreaY = (height * 68) / originalImageHeight;
                int clockAreaHeight = ((height * 96) / originalImageHeight) - clockAreaY;

                int nameAreaX = (width * 77) / originalImageWidth;
                int nameAreaWidth = ((width * 1000) / originalImageWidth) - nameAreaX;
                int nameAreaY = (height * 65) / originalImageHeight;
                int nameAreaHeight = ((height * 97) / originalImageHeight) - nameAreaY;

                int infoAreaX = (width * 77) / originalImageWidth;
                int infoAreaWidth = ((width * 1300) / originalImageWidth) - infoAreaX;
                int infoAreaY = (height * 96) / originalImageHeight;
                int infoAreaHeight = ((height * 125) / originalImageHeight) - infoAreaY;

                int searchStartX = (width * 420) / originalImageWidth;
                int searchEndX = (width * 1270) / originalImageWidth;
                int searchStartY = (height * 600) / originalImageHeight;
                int numberOfPixelsPerSearchLine = (width * (searchEndX - searchStartX)) / originalImageWidth;

                const int heightPerItem = 41;
                int itemHeight = (height * heightPerItem) / originalImageHeight;
                int capturedItemHeight = itemHeight - 10; //Reduce the height of the captured area by 10 pixels to avoid interference from the lines between the items

                int itemTopThreshold = ((height * 240) / originalImageHeight);
                int itemBottomThreshold = ((height * 950) / originalImageHeight);
                int rowTopThreshold = ((height * 249) / originalImageHeight);
                int rowBottomThreshold = ((height * 973) / originalImageHeight);

                return new ImageCoordinates(aspectRatio, new Rectangle(clockAreaX, clockAreaY, clockAreaWidth, clockAreaHeight), new Rectangle(nameAreaX, nameAreaY, nameAreaWidth, nameAreaHeight), new Rectangle(infoAreaX, infoAreaY, infoAreaWidth, infoAreaHeight), searchStartX, searchEndX, searchStartY, numberOfPixelsPerSearchLine, itemHeight, capturedItemHeight, itemTopThreshold, itemBottomThreshold, rowTopThreshold, rowBottomThreshold);
            }

            if (aspectRatio == _a16_10)
            {
                const int originalImageWidth = 1920;
                const int originalImageHeight = 1200;

                int clockAreaX = (width * 1690) / originalImageWidth;
                int clockAreaWidth = ((width * 1822) / originalImageWidth) - clockAreaX;
                int clockAreaY = (height * 139) / originalImageHeight;
                int clockAreaHeight = ((height * 164) / originalImageHeight) - clockAreaY;

                int nameAreaX = (width * 95) / originalImageWidth;
                int nameAreaWidth = ((width * 1020) / originalImageWidth) - nameAreaX;
                int nameAreaY = (height * 135) / originalImageHeight;
                int nameAreaHeight = ((height * 165) / originalImageHeight) - nameAreaY;

                int infoAreaX = (width * 95) / originalImageWidth;
                int infoAreaWidth = ((width * 1270) / originalImageWidth) - infoAreaX;
                int infoAreaY = (height * 164) / originalImageHeight;
                int infoAreaHeight = ((height * 191) / originalImageHeight) - infoAreaY;

                int searchStartX = (width * 417) / originalImageWidth;
                int searchEndX = (width * 1270) / originalImageWidth;
                int searchStartY = (height * 670) / originalImageHeight;
                int numberOfPixelsPerSearchLine = (width * (searchEndX - searchStartX)) / originalImageWidth;

                const int heightPerItem = 40;
                int itemHeight = (height * heightPerItem) / originalImageHeight;
                int capturedItemHeight = itemHeight - 10; //Reduce the height of the captured area by 10 pixels to avoid interference from the lines between the items

                int itemTopThreshold = ((height * 306) / originalImageHeight);
                int itemBottomThreshold = ((height * 1000) / originalImageHeight);
                int rowTopThreshold = ((height * 315) / originalImageHeight);
                int rowBottomThreshold = ((height * 1024) / originalImageHeight);

                return new ImageCoordinates(aspectRatio, new Rectangle(clockAreaX, clockAreaY, clockAreaWidth, clockAreaHeight), new Rectangle(nameAreaX, nameAreaY, nameAreaWidth, nameAreaHeight), new Rectangle(infoAreaX, infoAreaY, infoAreaWidth, infoAreaHeight), searchStartX, searchEndX, searchStartY, numberOfPixelsPerSearchLine, itemHeight, capturedItemHeight, itemTopThreshold, itemBottomThreshold, rowTopThreshold, rowBottomThreshold);
            }

            throw new Exception(string.Format("Unsupported aspect ratio ({0}). Please send in your screenshot and we can take a look at adding this aspect ratio.", aspectRatio));
        }

        public static ItemCoordinates GetItemCoordinates(int middleRowStart, ImageCoordinates imageCoordinates)
        {
            int width = imageCoordinates.AspectRatio.Width;
            int height = imageCoordinates.AspectRatio.Height;

            const int totalNumberOfRows = 19;

            if (imageCoordinates.AspectRatio == _a16_9)
            {
                const int originalImageWidth = 1920;
                const int originalImageHeight = 1080;


                int maxAllowedItemY = (height * 984) / originalImageHeight;
                int numberOfItemsBelow = (maxAllowedItemY - middleRowStart) / imageCoordinates.ItemHeight;
                int numberOfItemsAbove = totalNumberOfRows - numberOfItemsBelow;
                int firstItemY = middleRowStart - (numberOfItemsAbove * imageCoordinates.ItemHeight);

                int commodityNameXOffset = ((width * 83) / originalImageWidth) * 4;
                int commodityNameWidth = ((width * 355) / originalImageWidth);
                int sellPriceXOffset = ((width * 448) / originalImageWidth) * 4;
                int sellPriceWidth = ((width * 81) / originalImageWidth);
                int buyPriceXOffset = ((width * 536) / originalImageWidth) * 4;
                int buyPriceWidth = ((width * 81) / originalImageWidth);
                int demandXOffset = ((width * 715) / originalImageWidth) * 4;
                int demandWidth = ((width * 173) / originalImageWidth);
                int supplyXOffset = ((width * 900) / originalImageWidth) * 4;
                int supplyWidth = ((width * 194) / originalImageWidth);
                int galacticAverageXOffset = ((width * 1100) / originalImageWidth) * 4;
                int galacticAverageWidth = ((width * 183) / originalImageWidth);

                return new ItemCoordinates(commodityNameXOffset, commodityNameWidth, sellPriceXOffset, sellPriceWidth,
                    buyPriceXOffset, buyPriceWidth, demandXOffset, demandWidth, supplyXOffset, supplyWidth,
                    galacticAverageXOffset, galacticAverageWidth, firstItemY, totalNumberOfRows);
            }

            if (imageCoordinates.AspectRatio == _a16_10)
            {
                const int originalImageWidth = 1920;
                const int originalImageHeight = 1200;


                int maxAllowedItemY = (height * 1037) / originalImageHeight;
                int numberOfItemsBelow = (maxAllowedItemY - middleRowStart) / imageCoordinates.ItemHeight;
                int numberOfItemsAbove = totalNumberOfRows - numberOfItemsBelow;
                int firstItemY = middleRowStart - (numberOfItemsAbove * imageCoordinates.ItemHeight);

                int commodityNameXOffset = ((width * 99) / originalImageWidth) * 4;
                int commodityNameWidth = ((width * 349) / originalImageWidth);
                int sellPriceXOffset = ((width * 458) / originalImageWidth) * 4;
                int sellPriceWidth = ((width * 79) / originalImageWidth);
                int buyPriceXOffset = ((width * 543) / originalImageWidth) * 4;
                int buyPriceWidth = ((width * 79) / originalImageWidth);
                int demandXOffset = ((width * 720) / originalImageWidth) * 4;
                int demandWidth = ((width * 172) / originalImageWidth);
                int supplyXOffset = ((width * 900) / originalImageWidth) * 4;
                int supplyWidth = ((width * 190) / originalImageWidth);
                int galacticAverageXOffset = ((width * 1097) / originalImageWidth) * 4;
                int galacticAverageWidth = ((width * 178) / originalImageWidth);

                return new ItemCoordinates(commodityNameXOffset, commodityNameWidth, sellPriceXOffset, sellPriceWidth,
                    buyPriceXOffset, buyPriceWidth, demandXOffset, demandWidth, supplyXOffset, supplyWidth,
                    galacticAverageXOffset, galacticAverageWidth, firstItemY, totalNumberOfRows);
            }

            throw new Exception(string.Format("Unsupported aspect ratio ({0}). Please send in your screenshot and we can take a look at adding this aspect ratio.", imageCoordinates.AspectRatio));
        }
    }
}