using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using EliteTrader.EliteOcr.Enums;
using EliteTrader.EliteOcr.Interfaces;
using Tesseract;

namespace EliteTrader.EliteOcr.Tesseract
{
    public class OcrParser
    {
        private readonly ICommodityNameMatcher _commodityNameMatcher;

        private const int StationNameAttempts = 1;
        private const int DescriptionAttempts = 1;
        private const int CommodityNameAttempts = 1;
        private const int SellAttempts = 5;
        private const int BuyAttempts = 5;
        private const int DemandAttempts = 5;
        private const int GalacticAverageAttempts = 5;
        private const int ClockAttempts = 1;

        public OcrParser(ICommodityNameMatcher commodityNameMatcher)
        {
            _commodityNameMatcher = commodityNameMatcher;
        }

        public string ParseClock(Bitmap bitmap)
        {
            using (TesseractEngine engine = new TesseractEngine(null, "edc", EngineMode.TesseractOnly))
            {
                List<string> clockValues = TessHelper.Process(engine, bitmap, "0123456789:", "Station clock", ClockAttempts, false);
                string clock = TessHelper.MajorityVoteString(clockValues);

                return clock;
            }
        }

        public ParsedScreenshot Parse(ParsedScreenshotBitmaps parsedScreenshotBitmaps)
        {
            string clockStr = ParseClock(parsedScreenshotBitmaps.Clock);
            if (!IsCommoditiesScreen(clockStr))
            {
                throw new Exception(string.Format("Attempted to parse screen that is not a valid commodities screen. No clock found at the expected location in the upper right corner."));
            }

            string folderAboveTessData = GetFolderAboveTessData();

            List<CommodityItem> items = new List<CommodityItem>(parsedScreenshotBitmaps.ItemBitmapsList.Count);

            string stationName;
            string description;

            string configPath = Path.Combine(folderAboveTessData, @"tessdata\edl.config");

            using (TesseractEngine engine = new TesseractEngine(null, "edl", EngineMode.TesseractOnly, configPath))
            {
                //timestamp = TessHelper.Process(engine, timestampBitmap, "0123456789:JANFEBMRPYULGSOCTVD");
                List<string> stationNames = TessHelper.Process(engine, parsedScreenshotBitmaps.Name, "ABCDEFGHIJKLMNOPQRSTUVWXYZ,-'", "Station name", StationNameAttempts, true);
                stationName = TessHelper.MajorityVoteString(stationNames);
                List<string> descriptions = TessHelper.Process(engine, parsedScreenshotBitmaps.Description, "ABCDEFGHIJKLMNOPQRSTUVWXYZ,()", "Station description", DescriptionAttempts, true);
                description = TessHelper.MajorityVoteString(descriptions);

                foreach (CommodityItemBitmaps itemBitmaps in parsedScreenshotBitmaps.ItemBitmapsList)
                {
                    items.Add(ToItem(engine, itemBitmaps));
                }
            }

            StationDescription stationDescription = GetStationDescription(description);

            ParsedScreenshot parsedScreenshot = new ParsedScreenshot(stationName, stationDescription, items);

            return parsedScreenshot;
        }

        public CommodityItem ToItem(TesseractEngine engine, CommodityItemBitmaps itemBitmaps)
        {
            CommodityName name = GetCommodityName(engine, itemBitmaps.Name);
            EnumSupplyStatus demandStatus;
            int? demand = GetDemand(itemBitmaps.Demand, engine, name, out demandStatus);

            int? sell = GetSell(engine, name, itemBitmaps.Sell, demand != null);

            EnumSupplyStatus supplyStatus;
            int? supply = GetDemand(itemBitmaps.Supply, engine, name, out supplyStatus);
            
            int? buy = GetBuy(engine, name, itemBitmaps.Buy, supply != null);
            
            int galacticAverage = GetGalacticAverage(engine, name, itemBitmaps.GalacticAverage);

            return new CommodityItem(name.Name, name.RareName, sell, buy, demand, demandStatus, supply,
                supplyStatus, galacticAverage);
        }

        public static bool IsCommoditiesScreen(string clockStr)
        {
            string[] split = clockStr.Split(':');
            if (split.Length != 3)
            {
                return false;
            }
            for (int i = 0; i < 3; ++i)
            {
                string part = split[i];
                int n;
                if (!int.TryParse(part, out n))
                {
                    return false;
                }
                if (i == 0)
                {
                    if (n > 24)
                    {
                        return false;
                    }
                }
                else
                {
                    if (n > 60)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private int? GetDemand(PartialItemBitmap partialItemBitmap, TesseractEngine engine, CommodityName name, out EnumSupplyStatus status)
        {
            status = EnumSupplyStatus.None;
            if (partialItemBitmap == null)
            {
                return null;
            }
            Bitmap bitmap = partialItemBitmap.Bitmap;
            if (bitmap == null)
            {
                return null;
            }
            List<string> results = TessHelper.Process(engine, bitmap, "0123456789,HIGHMEDLOW",
                string.Format("Demand on commodity item ({0})", name), DemandAttempts, false);

            if (results.Count == 0)
            {
                throw new Exception(string.Format("Got no results when doing ocr for demand/supply on item ({0})", name));
                //bitmap.Save(@"c:\tmp\screens\failed.tif");
            }

            Dictionary<EnumSupplyStatus, int> votes = new Dictionary<EnumSupplyStatus, int>();
            List<string> resultsWithoutSizeIndicator = new List<string>();
            foreach (string str in results)
            {
                string result = str;
                EnumSupplyStatus statusVote;
                if (string.IsNullOrEmpty(result))
                {
                    continue;
                }
                result = result.ToLower();
                string resultWithoutSizeIndicator;
                if (result.Contains("high"))
                {
                    resultWithoutSizeIndicator = result.Substring(0, result.Length - 4).Trim();
                    statusVote = EnumSupplyStatus.High;
                }
                else if (result.Contains("med"))
                {
                    resultWithoutSizeIndicator = result.Substring(0, result.Length - 3).Trim();
                    statusVote = EnumSupplyStatus.Medium;
                }
                else if (result.Contains("low"))
                {
                    resultWithoutSizeIndicator = result.Substring(0, result.Length - 3).Trim();
                    statusVote = EnumSupplyStatus.Low;
                }
                else
                {
                    resultWithoutSizeIndicator = null;
                    statusVote = EnumSupplyStatus.None;
                }
                resultsWithoutSizeIndicator.Add(resultWithoutSizeIndicator);

                int value;
                if (votes.TryGetValue(statusVote, out value))
                {
                    votes[statusVote] = ++value;
                }
                else
                {
                    votes.Add(statusVote, 1);
                }
            }
            status = votes.OrderByDescending(a => a.Value).First().Key;
            return GetPrice(resultsWithoutSizeIndicator, name);
        }

        private CommodityName GetCommodityName(TesseractEngine engine, PartialItemBitmap partialItemBitmap)
        {
            if (partialItemBitmap == null)
            {
                throw new Exception(string.Format("Name bitmap must be set"));
            }
            Bitmap bitmap = partialItemBitmap.Bitmap;
            if (bitmap == null)
            {
                throw new Exception(string.Format("Name bitmap must be set"));
            }
            List<string> results = TessHelper.Process(engine, bitmap, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ.-'", string.Format("Name on commodity item"), CommodityNameAttempts, true);

            string result = TessHelper.MajorityVoteString(results);

            CommodityName name = _commodityNameMatcher.FromString(result);
            if (name.Name == EnumCommodityItemName.Unknown)
            {
                throw new Exception(string.Format("Failed to match name ({0}) to a known commodity", result));
            }

            return name;
        }

        //private int? GetSell(TesseractEngine engine, CommodityName name)
        //{
        //    if (Sell.BitmapChars == null)
        //    {
        //        return null;
        //    }

        //    return GetPriceFromCharacterArray(engine, name, Sell.BitmapChars);
        //}

        //private int? GetPriceFromCharacterArray(TesseractEngine engine, CommodityName name, Bitmap[] characterBitmaps)
        //{
        //    TessHelper.SetVariable(engine, "tessedit_char_whitelist", "0123456789");
        //    PageSegMode previousValue = engine.DefaultPageSegMode;
        //    engine.DefaultPageSegMode = PageSegMode.SingleChar;
        //    string result = "";
        //    foreach (Bitmap bitmapChar in Sell.BitmapChars)
        //    {
        //        using (Page page = engine.Process(bitmapChar))
        //        {
        //            string str = page.GetText().Trim();
        //            int n;
        //            if (!int.TryParse(str, out n))
        //            {
        //                bitmapChar.Save(@"c:\tmp\screens\potet2.tif");
        //                throw new Exception(string.Format("Unexpected character when parsing price ({0}) for commodity item ({1})", str, name));
        //            }
        //            result += str;
        //        }
        //    }
        //    engine.DefaultPageSegMode = previousValue;

        //    return GetPrice(result, name);
        //}

        //private int? GetBuy(TesseractEngine engine, CommodityName name)
        //{
        //    if (Buy.BitmapChars == null)
        //    {
        //        return null;
        //    }

        //    return GetPriceFromCharacterArray(engine, name, Buy.BitmapChars);
        //}

        private int? GetSell(TesseractEngine engine, CommodityName name, PartialItemBitmap partialItemBitmap, bool mustHaveValue)
        {
            if (partialItemBitmap == null)
            {
                return null;
            }
            Bitmap bitmap = partialItemBitmap.Bitmap;
            if (bitmap == null)
            {
                return null;
            }

            List<string> results = TessHelper.Process(engine, bitmap, "0123456789,", string.Format("Sell on commodity item ({0})", name), SellAttempts, mustHaveValue);

            return GetPrice(results, name);
        }

        private int? GetBuy(TesseractEngine engine, CommodityName name, PartialItemBitmap partialItemBitmap, bool mustHaveValue)
        {
            if (partialItemBitmap == null)
            {
                return null;
            }
            Bitmap bitmap = partialItemBitmap.Bitmap;
            if (bitmap == null)
            {
                return null;
            }

            List<string> results = TessHelper.Process(engine, bitmap, "0123456789,", string.Format("Buy on commodity item ({0})", name), BuyAttempts, mustHaveValue);

            return GetPrice(results, name);
        }

        private int GetGalacticAverage(TesseractEngine engine, CommodityName name, PartialItemBitmap partialItemBitmap)
        {
            if (partialItemBitmap == null)
            {
                throw new Exception(string.Format("Galactic average must be set"));
            }
            Bitmap bitmap = partialItemBitmap.Bitmap;
            if (bitmap == null)
            {
                throw new Exception(string.Format("Galactic average must be set"));
            }
            List<string> results = TessHelper.Process(engine, bitmap, "0123456789,CR", string.Format("GalacticAverage on commodity item ({0})", name), GalacticAverageAttempts, true);

            int? i = GetPrice(results, name);

            if (!i.HasValue)
            {
                throw new Exception(
                    string.Format(
                        "Failed to parse galactic average from strings ({0}) on commoditiy item ({1})", string.Join(", ", results),
                        name));
            }
            return i.Value;
        }

        private int? GetPrice(List<string> resultsWithoutSizeIndicator, CommodityName name)
        {
            if (resultsWithoutSizeIndicator.Count == 0)
            {
                return null;
            }

            Dictionary<int, int> votes = new Dictionary<int, int>();
            foreach(string resultWithoutSizeIndicator in resultsWithoutSizeIndicator)
            {
                if (string.IsNullOrEmpty(resultWithoutSizeIndicator))
                {
                    continue;
                }

                string result = new string(resultWithoutSizeIndicator.ToLower().Replace("o", "0").Replace("d", "0").Replace("i", "1").Where(Char.IsDigit).ToArray()).Trim();
                if (string.IsNullOrEmpty(result))
                {
                    continue;
                }
                int price;
                if (!int.TryParse(result, out price))
                {
                    throw new Exception(string.Format("Failed to parse price string ({0}) to a valid integer on commodity ({1})", resultWithoutSizeIndicator, name));
                }

                int value;
                if (votes.TryGetValue(price, out value))
                {
                    votes[price] = ++value;
                }
                else
                {
                    votes.Add(price, 1);
                }
            }
            if (votes.Count == 0)
            {
                return null;
            }

            return votes.OrderByDescending(a => a.Value).FirstOrDefault().Key;
        }

        private StationDescription GetStationDescription(string description)
        {
            description = description.ToLower();
            //string str = "poor, large population agriculture economy (independent, anarchy)";

            EnumStationWealth stationWealth = EnumStationWealth.Normal;
            if (description.Contains("poor"))
            {
                stationWealth = EnumStationWealth.Poor;
            }
            else if (description.Contains("wealthy"))
            {
                stationWealth = EnumStationWealth.Wealthy;
            }

            EnumPopulationSize populationSize = EnumPopulationSize.Unknown;
            if (description.Contains("tiny"))
            {
                populationSize = EnumPopulationSize.Tiny;
            }
            else if (description.Contains("small"))
            {
                populationSize = EnumPopulationSize.Small;
            }
            else if (description.Contains("medium"))
            {
                populationSize = EnumPopulationSize.Medium;
            }
            else if (description.Contains("very large"))
            {
                populationSize = EnumPopulationSize.VeryLarge;
            }
            else if (description.Contains("large"))
            {
                populationSize = EnumPopulationSize.Large;
            }
            else if (description.Contains("huge"))
            {
                populationSize = EnumPopulationSize.Huge;
            }

            StationEconomy primaryStationEconomyObject = GetStationEconomy(description, 0);
            EnumStationEconomy primaryStationEconomy = primaryStationEconomyObject.EconomyType;
            EnumStationEconomy? secondaryStationEconomy = null;
            if (primaryStationEconomyObject.EconomyType != primaryStationEconomy)
            {
                secondaryStationEconomy = GetStationEconomy(description, primaryStationEconomyObject.IndexInDescription).EconomyType;
            }

            EnumAllegiance allegiance = EnumAllegiance.Unknown;
            if (description.Contains("alliance"))
            {
                allegiance = EnumAllegiance.Alliance;
            }
            else if (description.Contains("empire"))
            {
                allegiance = EnumAllegiance.Empire;
            }
            else if (description.Contains("federation"))
            {
                allegiance = EnumAllegiance.Federation;
            }
            else if (description.Contains("independent"))
            {
                allegiance = EnumAllegiance.Independent;
            }

            EnumGovernment government = EnumGovernment.Unknown;
            if (description.Contains("anarchy"))
            {
                government = EnumGovernment.Anarchy;
            }
            else if (description.Contains("colony"))
            {
                government = EnumGovernment.Colony;
            }
            else if (description.Contains("communism"))
            {
                government = EnumGovernment.Communism;
            }
            else if (description.Contains("confederacy"))
            {
                government = EnumGovernment.Confederacy;
            }
            else if (description.Contains("cooperative"))
            {
                government = EnumGovernment.Cooperative;
            }
            else if (description.Contains("corporate"))
            {
                government = EnumGovernment.Corporate;
            }
            else if (description.Contains("democracy"))
            {
                government = EnumGovernment.Democracy;
            }
            else if (description.Contains("dictatorship"))
            {
                government = EnumGovernment.Dictatorship;
            }
            else if (description.Contains("feudal"))
            {
                government = EnumGovernment.Feudal;
            }
            else if (description.Contains("imperial"))
            {
                government = EnumGovernment.Imperial;
            }
            else if (description.Contains("patronage"))
            {
                government = EnumGovernment.Patronage;
            }
            else if (description.Contains("prison"))
            {
                government = EnumGovernment.PrisonColony;
            }
            else if (description.Contains("theocracy"))
            {
                government = EnumGovernment.Theocracy;
            }

            return new StationDescription(stationWealth, populationSize, primaryStationEconomy, secondaryStationEconomy, allegiance, government);
        }

        private static StationEconomy GetStationEconomy(string description, int searchFromIndex)
        {
            int stationEconomyIndex;
            if ((stationEconomyIndex = description.IndexOf("agriculture", searchFromIndex, StringComparison.Ordinal)) >= 0)
            {
                return new StationEconomy(EnumStationEconomy.Agricultural, stationEconomyIndex);
            }
            if ((stationEconomyIndex = description.IndexOf("industrial", searchFromIndex, StringComparison.Ordinal)) >= 0)
            {
                return new StationEconomy(EnumStationEconomy.Industrial, stationEconomyIndex);
            }
            if ((stationEconomyIndex = description.IndexOf("extraction", searchFromIndex, StringComparison.Ordinal)) >= 0)
            {
                return new StationEconomy(EnumStationEconomy.Extraction, stationEconomyIndex);
            }
            if ((stationEconomyIndex = description.IndexOf("tech", searchFromIndex, StringComparison.Ordinal)) >= 0)
            {
                return new StationEconomy(EnumStationEconomy.HighTech, stationEconomyIndex);
            }
            if ((stationEconomyIndex = description.IndexOf("military", searchFromIndex, StringComparison.Ordinal)) >= 0)
            {
                return new StationEconomy(EnumStationEconomy.Millitary, stationEconomyIndex);
            }
            if ((stationEconomyIndex = description.IndexOf("refinery", searchFromIndex, StringComparison.Ordinal)) >= 0)
            {
                return new StationEconomy(EnumStationEconomy.Refinery, stationEconomyIndex);
            }
            if ((stationEconomyIndex = description.IndexOf("service", searchFromIndex, StringComparison.Ordinal)) >= 0)
            {
                return new StationEconomy(EnumStationEconomy.Service, stationEconomyIndex);
            }
            if ((stationEconomyIndex = description.IndexOf("terraforming", searchFromIndex, StringComparison.Ordinal)) >= 0)
            {
                return new StationEconomy(EnumStationEconomy.Terraforming, stationEconomyIndex);
            }
            if ((stationEconomyIndex = description.IndexOf("tourism", searchFromIndex, StringComparison.Ordinal)) >= 0)
            {
                return new StationEconomy(EnumStationEconomy.Tourism, stationEconomyIndex);
            }
            return new StationEconomy(EnumStationEconomy.Unknown, -1);
        }

        private class StationEconomy
        {
            public EnumStationEconomy EconomyType { get; private set; }
            public int IndexInDescription { get; private set; }

            public StationEconomy(EnumStationEconomy economyType, int indexInDescription)
            {
                EconomyType = economyType;
                IndexInDescription = indexInDescription;
            }
        }

        private static string GetFolderAboveTessData()
        {
            string folderAboveTessData = Environment.GetEnvironmentVariable("TESSDATA_PREFIX");
            if (string.IsNullOrEmpty(folderAboveTessData))
            {
                throw new Exception(string.Format("TESSDATA_PREFIX environment variable not set. Cannot continue."));
            }
            return folderAboveTessData;
        }
    }
}
