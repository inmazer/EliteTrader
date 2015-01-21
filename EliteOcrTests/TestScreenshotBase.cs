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
            Assert.AreEqual(expected.Allegiance, actual.Allegiance);
            Assert.AreEqual(expected.PrimaryEconomy, actual.PrimaryEconomy);
            Assert.AreEqual(expected.SecondaryEconomy, actual.SecondaryEconomy);
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