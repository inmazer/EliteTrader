using EliteTrader.EliteOcr;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EliteOcrTests
{
    public abstract class TestScreenshotBase
    {
        protected void Compare(ParsedScreenshot expected, ParsedScreenshot actual)
        {
            Assert.AreEqual(expected.StationName.ToLower(), actual.StationName.ToLower());
            CompareStationDescription(expected.Description, actual.Description);
            Assert.AreEqual(expected.Items.Count, actual.Items.Count);
            for (int i = 0; i < expected.Items.Count; ++i)
            {
                CompareCommodityItem(expected.Items[i], actual.Items[i]);
            }
        }

        private void CompareStationDescription(StationDescription expected, StationDescription actual)
        {
            Assert.AreEqual(expected.Allegance, actual.Allegance);
            Assert.AreEqual(expected.Economy, actual.Economy);
            Assert.AreEqual(expected.Government, actual.Government);
            Assert.AreEqual(expected.Population, actual.Population);
            Assert.AreEqual(expected.Wealth, actual.Wealth);
        }

        private void CompareCommodityItem(CommodityItem expected, CommodityItem actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.RareName != null ? expected.RareName.ToLower() : null, actual.RareName != null ? actual.RareName.ToLower() : null);
            Assert.AreEqual(expected.Sell, actual.Sell);
            Assert.AreEqual(expected.Buy, actual.Buy);
            Assert.AreEqual(expected.Demand, actual.Demand);
            Assert.AreEqual(expected.DemandStatus, actual.DemandStatus);
            Assert.AreEqual(expected.Supply, actual.Supply);
            Assert.AreEqual(expected.SupplyStatus, actual.SupplyStatus);
            Assert.AreEqual(expected.GalacticAverage, actual.GalacticAverage);
        }
    }
}