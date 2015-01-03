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

        public OcrParser(ICommodityNameMatcher commodityNameMatcher)
        {
            _commodityNameMatcher = commodityNameMatcher;
        }

        public ParsedScreenshot Parse(ParsedScreenshotBitmaps parsedScreenshotBitmaps)
        {
            string folderAboveTessData = Environment.GetEnvironmentVariable("TESSDATA_PREFIX");
            if (string.IsNullOrEmpty(folderAboveTessData))
            {
                throw new Exception(string.Format("TESSDATA_PREFIX environment variable not set. Cannot continue."));
            }

            List<CommodityItem> items = new List<CommodityItem>(parsedScreenshotBitmaps.ItemBitmapsList.Count);

            string stationName;
            string description;

            string configPath = Path.Combine(folderAboveTessData, @"tessdata\edl.config");

            using (TesseractEngine engine = new TesseractEngine(null, "edl", EngineMode.TesseractOnly, configPath))
            {
                //timestamp = TessHelper.Process(engine, timestampBitmap, "0123456789:JANFEBMRPYULGSOCTVD");
                string[] stationNames = TessHelper.Process(engine, parsedScreenshotBitmaps.Name, "ABCDEFGHIJKLMNOPQRSTUVWXYZ,-'", "Station name", StationNameAttempts);
                stationName = TessHelper.MajorityVoteString(stationNames);
                string[] descriptions = TessHelper.Process(engine, parsedScreenshotBitmaps.Description, "ABCDEFGHIJKLMNOPQRSTUVWXYZ,()", "Station description", DescriptionAttempts);
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

        private StationDescription GetStationDescription(string description)
        {
            description = description.ToLower();
            //string str = "poor, large population agriculture economy (independent, anarchy)";

            EnumStationWealth stationWealth = EnumStationWealth.Unknown;
            if (description.Contains("poor"))
            {
                stationWealth = EnumStationWealth.Poor;
            }

            EnumPopulationSize populationSize = EnumPopulationSize.Unknown;
            if (description.Contains("small"))
            {
                populationSize = EnumPopulationSize.Small;
            }
            else if (description.Contains("medium"))
            {
                populationSize = EnumPopulationSize.Medium;
            }
            else if (description.Contains("large"))
            {
                populationSize = EnumPopulationSize.Large;
            }

            EnumStationEconomy stationEconomy = EnumStationEconomy.Unknown;
            if(description.Contains("agriculture"))
            {
                stationEconomy = EnumStationEconomy.Agriculture;
            }

            EnumAllegance allegance = EnumAllegance.Unknown;
            if (description.Contains("alliance"))
            {
                allegance = EnumAllegance.Alliance;
            }
            else if (description.Contains("empire"))
            {
                allegance = EnumAllegance.Empire;
            }
            else if (description.Contains("federation"))
            {
                allegance = EnumAllegance.Federation;
            }
            else if (description.Contains("independent"))
            {
                allegance = EnumAllegance.Independent;
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

            return new StationDescription(stationWealth, populationSize, stationEconomy, allegance, government);
        }

        public CommodityItem ToItem(TesseractEngine engine, CommodityItemBitmaps itemBitmaps)
        {
            CommodityName name = GetCommodityName(engine, itemBitmaps.Name);
            int? sell = GetSell(engine, name, itemBitmaps.Sell);
            int? buy = GetBuy(engine, name, itemBitmaps.Buy);
            EnumSupplyStatus demandStatus;
            int? demand = GetDemand(itemBitmaps.Demand, engine, name, out demandStatus);

            EnumSupplyStatus supplyStatus;
            int? supply = GetDemand(itemBitmaps.Supply, engine, name, out supplyStatus);

            int galacticAverage = GetGalacticAverage(engine, name, itemBitmaps.GalacticAverage);

            return new CommodityItem(name.Name, name.RareName, sell, buy, demand, demandStatus, supply,
                supplyStatus, galacticAverage);
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
            string[] results = TessHelper.Process(engine, bitmap, "0123456789,HIGHMEDLOW",
                string.Format("Demand on commodity item ({0})", name), DemandAttempts);

            Dictionary<EnumSupplyStatus, int> votes = new Dictionary<EnumSupplyStatus, int>();
            List<string> resultsWithoutSizeIndicator = new List<string>();
            for(int i = 0; i < results.Length; ++i)
            {
                EnumSupplyStatus statusVote;
                string result = results[i];
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
            return GetPrice(resultsWithoutSizeIndicator.ToArray(), name);
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
            string[] results = TessHelper.Process(engine, bitmap, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ.-'", string.Format("Name on commodity item"), CommodityNameAttempts);

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

        private int? GetSell(TesseractEngine engine, CommodityName name, PartialItemBitmap partialItemBitmap)
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

            string[] results = TessHelper.Process(engine, bitmap, "0123456789,", string.Format("Sell on commodity item ({0})", name), SellAttempts);

            return GetPrice(results, name);
        }

        private int? GetBuy(TesseractEngine engine, CommodityName name, PartialItemBitmap partialItemBitmap)
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

            string[] results = TessHelper.Process(engine, bitmap, "0123456789,", string.Format("Buy on commodity item ({0})", name), BuyAttempts);

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
            string[] results = TessHelper.Process(engine, bitmap, "0123456789,CR", string.Format("GalacticAverage on commodity item ({0})", name), GalacticAverageAttempts);

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

        private int? GetPrice(string[] resultsWithoutSizeIndicator, CommodityName name)
        {
            if (resultsWithoutSizeIndicator.Length == 0)
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
    }
}
