using System.IO;
using EliteTrader.EliteOcr;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EliteOcrTests
{
    public abstract class TestScreenshotBase
    {
        protected string Serialize(object o)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter { AllowIntegerValues = false });
            string str = JsonConvert.SerializeObject(o, Formatting.Indented, settings);
            return str;
        }

        protected ParsedScreenshot Deserialize(string path)
        {
            return JsonConvert.DeserializeObject<ParsedScreenshot>(File.ReadAllText(path));
        }

        protected void Compare(string path, ParsedScreenshot actual)
        {
            ParsedScreenshot expected = Deserialize(path);
            Compare(expected, actual);
        }

        protected void Compare(ParsedScreenshot expected, ParsedScreenshot actual)
        {
            Assert.AreEqual(expected.StationName.ToLower(), actual.StationName.ToLower(), "On station name");
            CompareStationDescription(expected.Description, actual.Description, actual.StationName);
            Assert.AreEqual(expected.Items.Count, actual.Items.Count, "Items count");
            for (int i = 0; i < expected.Items.Count; ++i)
            {
                CompareCommodityItem(expected.Items[i], actual.Items[i]);
            }
        }

        private void CompareStationDescription(StationDescription expected, StationDescription actual, string stationName)
        {
            Assert.AreEqual(expected.Allegiance, actual.Allegiance, string.Format("On Allegiance for station ({0})", stationName));
            Assert.AreEqual(expected.PrimaryEconomy, actual.PrimaryEconomy, string.Format("On PrimaryEconomy for station ({0})", stationName));
            Assert.AreEqual(expected.SecondaryEconomy, actual.SecondaryEconomy, string.Format("On SecondaryEconomy for station ({0})", stationName));
            Assert.AreEqual(expected.Government, actual.Government, string.Format("On Government for station ({0})", stationName));
            Assert.AreEqual(expected.Population, actual.Population, string.Format("On Population for station ({0})", stationName));
            Assert.AreEqual(expected.Wealth, actual.Wealth, string.Format("On Wealth for station ({0})", stationName));
        }

        private void CompareCommodityItem(CommodityItem expected, CommodityItem actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.RareName != null ? expected.RareName.ToLower() : null, actual.RareName != null ? actual.RareName.ToLower() : null, string.Format("On RareName for commodity ({0})", actual.Name));
            Assert.AreEqual(expected.Sell, actual.Sell, string.Format("On Sell for commodity ({0})", actual.Name));
            Assert.AreEqual(expected.Buy, actual.Buy, string.Format("On Buy for commodity ({0})", actual.Name));
            Assert.AreEqual(expected.Demand, actual.Demand, string.Format("On Demand for commodity ({0})", actual.Name));
            Assert.AreEqual(expected.DemandStatus, actual.DemandStatus, string.Format("On DemandStatus for commodity ({0})", actual.Name));
            Assert.AreEqual(expected.Supply, actual.Supply, string.Format("On Supply for commodity ({0})", actual.Name));
            Assert.AreEqual(expected.SupplyStatus, actual.SupplyStatus, string.Format("On SupplyStatus for commodity ({0})", actual.Name));
            Assert.AreEqual(expected.GalacticAverage, actual.GalacticAverage, string.Format("On GalacticAverage for commodity ({0})", actual.Name));
        }
    }
}