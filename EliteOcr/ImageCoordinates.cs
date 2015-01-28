using System.Drawing;

namespace EliteTrader.EliteOcr
{
    public class ImageCoordinates
    {
        public const int TotalNumberOfRows = 19;

        public int ScreenshotWidth { get; private set; }
        public int ScreenshotHeight { get; private set; }

        public int GridLeftX { get; private set; }
        public int GridRightX { get; private set; }
        public int GridLowerY { get; private set; }
        public int GridUpperY { get; private set; }
        public int ClockRightX { get; private set; }
        public int StationNameUpperY { get; private set; }
        public int StationNameLowerY { get; private set; }
        public int StationDescriptionUpperY { get; private set; }
        public int StationDescriptionLowerY { get; private set; }
        
        public int CommodityItemNameStartX { get; private set; }
        public int CommodityItemNameEndX { get; private set; }
        public int SellStartX { get; private set; }
        public int SellEndX { get; private set; }
        public int BuyStartX { get; private set; }
        public int BuyEndX { get; private set; }
        public int CargoStartX { get; private set; }
        public int CargoEndX { get; private set; }
        public int DemandStartX { get; private set; }
        public int DemandEndX { get; private set; }
        public int SupplyStartX { get; private set; }
        public int SupplyEndX { get; private set; }
        public int GalacticAverageStartX { get; private set; }
        public int GalacticAverageEndX { get; private set; }

        public int FirstItemY { get; private set; }
        public decimal RowHeight { get; private set; }

        public ImageCoordinates(int screenshotWidth, int screenshotHeight, int gridLeftX, int gridRightX, int gridLowerY, int gridUpperY, int clockRightX, int stationNameUpperY, int stationNameLowerY, int stationDescriptionUpperY, int stationDescriptionLowerY, int commodityItemNameStartX, int commodityItemNameEndX, int sellStartX, int sellEndX, int buyStartX, int buyEndX, int cargoStartX, int cargoEndX, int demandStartX, int demandEndX, int supplyStartX, int supplyEndX, int galacticAverageStartX, int galacticAverageEndX, int firstItemY, decimal rowHeight)
        {
            ScreenshotWidth = screenshotWidth;
            ScreenshotHeight = screenshotHeight;

            GridLeftX = gridLeftX;
            GridRightX = gridRightX;
            GridLowerY = gridLowerY;
            GridUpperY = gridUpperY;
            ClockRightX = clockRightX;
            StationNameUpperY = stationNameUpperY;
            StationNameLowerY = stationNameLowerY;
            StationDescriptionUpperY = stationDescriptionUpperY;
            StationDescriptionLowerY = stationDescriptionLowerY;
            CommodityItemNameStartX = commodityItemNameStartX;
            CommodityItemNameEndX = commodityItemNameEndX;
            SellStartX = sellStartX;
            SellEndX = sellEndX;
            BuyStartX = buyStartX;
            BuyEndX = buyEndX;
            CargoStartX = cargoStartX;
            CargoEndX = cargoEndX;
            DemandStartX = demandStartX;
            DemandEndX = demandEndX;
            SupplyStartX = supplyStartX;
            SupplyEndX = supplyEndX;
            GalacticAverageStartX = galacticAverageStartX;
            GalacticAverageEndX = galacticAverageEndX;
            FirstItemY = firstItemY;
            RowHeight = rowHeight;
        }

        public Rectangle NameRectangle
        {
            get
            {
                return new Rectangle(GridLeftX, StationNameUpperY, GridRightX - GridLeftX,
                    StationNameLowerY - StationNameUpperY);
            }
        }

        public Rectangle DescriptionRectangle
        {
            get
            {
                return new Rectangle(GridLeftX, StationDescriptionUpperY, GridRightX - GridLeftX,
                    StationDescriptionLowerY - StationDescriptionUpperY);
            }
        }

        public Rectangle ClockRectangle
        {
            get
            {
                int x = ClockRightX - ((ClockRightX - GridRightX) / 3);
                return new Rectangle(x, StationNameUpperY, ClockRightX - x,
                    StationNameLowerY - StationNameUpperY);
            }
        }

        public int CommodityNameWidth
        {
            get { return CommodityItemNameEndX - CommodityItemNameStartX; }
        }

        public int SellPriceWidth
        {
            get { return SellEndX - SellStartX; }
        }

        public int BuyPriceWidth
        {
            get { return BuyEndX - BuyStartX; }
        }

        public int DemandWidth
        {
            get { return DemandEndX - DemandStartX; }
        }

        public int SupplyWidth
        {
            get { return SupplyEndX - SupplyStartX; }
        }

        public int GalacticAverageWidth
        {
            get { return GalacticAverageEndX - GalacticAverageStartX; }
        }

        public int RowTopThreshold
        {
            get { return GridUpperY + 9; }
        }

        public int RowBottomThreshold
        {
            get
            {
                return GridLowerY - 4;
            }
        }
    }
}
