using System.Drawing;

namespace EliteTrader.EliteOcr
{
    public class ImageCoordinates
    {
        public AspectRatio AspectRatio { get; private set; }

        public Rectangle Clock { get; private set; }
        public Rectangle Name { get; private set; }
        public Rectangle Info { get; private set; }

        public int SearchStartX { get; private set; }
        public int SearchEndX { get; private set; }
        public int SearchStartY { get; private set; }
        public int NumberOfPixelsPerSearchLine { get; private set; }

        public int ItemHeight { get; private set; }

        public int CapturedItemHeight { get; private set; }
        public int ItemTopThreshold { get; private set; }
        public int ItemBottomThreshold { get; private set; }
        public int RowTopThreshold { get; private set; }
        public int RowBottomThreshold { get; private set; }

        public ImageCoordinates(AspectRatio aspectRatio, Rectangle clock, Rectangle name, Rectangle info, int searchStartX, int searchEndX, int searchStartY, int numberOfPixelsPerSearchLine, int itemHeight, int capturedItemHeight, int itemTopThreshold, int itemBottomThreshold, int rowTopThreshold, int rowBottomThreshold)
        {
            AspectRatio = aspectRatio;
            Clock = clock;
            Name = name;
            Info = info;
            SearchStartX = searchStartX;
            SearchEndX = searchEndX;
            SearchStartY = searchStartY;
            NumberOfPixelsPerSearchLine = numberOfPixelsPerSearchLine;
            ItemHeight = itemHeight;
            CapturedItemHeight = capturedItemHeight;
            ItemTopThreshold = itemTopThreshold;
            ItemBottomThreshold = itemBottomThreshold;
            RowTopThreshold = rowTopThreshold;
            RowBottomThreshold = rowBottomThreshold;
        }
    }

    public class ItemCoordinates
    {
        public int CommodityNameXOffset { get; private set; }
        public int CommodityNameWidth { get; private set; }
        public int SellPriceXOffset { get; private set; }
        public int SellPriceWidth { get; private set; }
        public int BuyPriceXOffset { get; private set; }
        public int BuyPriceWidth { get; private set; }
        public int DemandXOffset { get; private set; }
        public int DemandWidth { get; private set; }
        public int SupplyXOffset { get; private set; }
        public int SupplyWidth { get; private set; }
        public int GalacticAverageXOffset { get; private set; }
        public int GalacticAverageWidth { get; private set; }

        public int FirstItemY { get; private set; }
        public int TotalNumberOfRows { get; private set; }

        public ItemCoordinates(int commodityNameXOffset, int commodityNameWidth, int sellPriceXOffset, int sellPriceWidth, int buyPriceXOffset, int buyPriceWidth, int demandXOffset, int demandWidth, int supplyXOffset, int supplyWidth, int galacticAverageXOffset, int galacticAverageWidth, int firstItemY, int totalNumberOfRows)
        {
            CommodityNameXOffset = commodityNameXOffset;
            CommodityNameWidth = commodityNameWidth;
            SellPriceXOffset = sellPriceXOffset;
            SellPriceWidth = sellPriceWidth;
            BuyPriceXOffset = buyPriceXOffset;
            BuyPriceWidth = buyPriceWidth;
            DemandXOffset = demandXOffset;
            DemandWidth = demandWidth;
            SupplyXOffset = supplyXOffset;
            SupplyWidth = supplyWidth;
            GalacticAverageXOffset = galacticAverageXOffset;
            GalacticAverageWidth = galacticAverageWidth;

            FirstItemY = firstItemY;
            TotalNumberOfRows = totalNumberOfRows;
        }
    }
}
