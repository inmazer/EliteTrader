using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EliteTrader.EliteOcr.Enums;

namespace EliteTrader.EliteOcr
{
    public enum EnumStationWealth
    {
        Poor = 1,
        Normal = 2,
        Wealthy = 3,
    }

    public enum EnumPopulationSize
    {
        Unknown = 0,
        Tiny = 1,
        Small = 2,
        Medium = 3,
        Large = 4,
        VeryLarge = 5,
        Huge = 5
    }

    public enum EnumStationEconomy
    {
        Unknown = 0,
        Agricultural = 1,
        Extraction = 2,
        HighTech = 3,
        Industrial = 4,
        Refinery = 5,
        Service = 6,
        None = 7,
        Tourism = 8,
        Terraforming = 9,
        Millitary = 10,
    }

    public enum EnumAllegiance
    {
        Unknown = 0,
        Alliance = 1,
        Empire = 2,
        Federation = 3,
        Independent = 4,
        None = 5,
    }

    public enum EnumGovernment
    {
        Unknown = 0,
        Anarchy = 1,
        Colony = 2,
        Communism = 3,
        Corporate = 4,
        Democracy = 5,
        None = 6,
        Feudal = 7,
        Dictatorship = 8,
        Theocracy = 9,
        Imperial = 10,
        Confederacy = 11,
        Patronage = 12,
        Cooperative = 13,
        PrisonColony = 14,
    }

    public class StationDescription
    {
        public EnumStationWealth Wealth { get; private set; }
        public EnumPopulationSize Population { get; private set; }
        public EnumStationEconomy PrimaryEconomy { get; private set; }
        public EnumStationEconomy? SecondaryEconomy { get; private set; }
        public EnumAllegiance Allegiance { get; private set; }
        public EnumGovernment Government { get; private set; }

        public StationDescription(EnumStationWealth wealth, EnumPopulationSize population, EnumStationEconomy primaryEconomy, EnumStationEconomy? secondaryEconomy, EnumAllegiance allegiance, EnumGovernment government)
        {
            Wealth = wealth;
            Population = population;
            PrimaryEconomy = primaryEconomy;
            SecondaryEconomy = secondaryEconomy;
            Allegiance = allegiance;
            Government = government;
        }
    }

    public class ParsedScreenshot
    {
        //public DateTime Timestamp { get; private set; }
        public string StationName { get; private set; }
        public StationDescription Description { get; private set; }

        public List<CommodityItem> Items { get; private set; }

        public ParsedScreenshot(string stationName, StationDescription description, List<CommodityItem> items)
        {
            //Timestamp = timestamp;
            StationName = stationName;
            Description = description;
            Items = items;
        }

        public void UpdateStationName(string stationName)
        {
            StationName = stationName;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine(string.Format("Timestamp: {0}", Timestamp));
            sb.AppendLine(string.Format("Station: {0}", StationName));
            sb.AppendLine(string.Format("Description: {0}", Description));
            sb.AppendLine("---------------------------------------------------------------------------------------------------");
            sb.AppendLine(" Goods                         | Sell    | Buy     | Demand       | Supply       | Galactic average");
            sb.AppendLine("---------------------------------------------------------------------------------------------------");

            foreach (CommodityItem item in Items)
            {
                sb.AppendLine(item.ToString());
            }

            return sb.ToString();
        }

        public void Add(ParsedScreenshot other)
        {
            if (string.Compare(StationName, other.StationName, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                throw new Exception(string.Format("Tried to combine parsed screenshots from two different stations. ({0}) vs ({1})", StationName, other.StationName));
            }

            List<string> rareNames = Items.Where(a => a.Name == EnumCommodityItemName.Rare).Select(a => a.RareName.ToLower()).ToList();

            foreach (CommodityItem otherItem in other.Items)
            {
                if (otherItem.Name == EnumCommodityItemName.Rare)
                {
                    if (!rareNames.Contains(otherItem.RareName.ToLower()))
                    {
                        Items.Add(otherItem);
                    }
                    continue;
                }

                if (Items.All(a => a.Name != otherItem.Name))
                {
                    Items.Add(otherItem);
                }
            }
        }
    }
}