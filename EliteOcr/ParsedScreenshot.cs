using System.Collections.Generic;
using System.Text;

namespace EliteTrader.EliteOcr
{
    public enum EnumStationWealth
    {
        Unknown = 0,
        Poor = 1,
        Normal = 2, //Unknown name
        Rich = 3,
    }

    public enum EnumPopulationSize
    {
        Unknown = 0,
        Small = 1,
        Medium = 2,
        Large = 3
    }

    public enum EnumStationEconomy
    {
        Unknown = 0,
        Agriculture = 1,

    }

    public enum EnumAllegance
    {
        Unknown = 0,
        Alliance = 1,
        Empire = 2,
        Federation = 3,
        Independent = 4
    }

    public enum EnumGovernment
    {
        Unknown = 0,
        Anarchy = 1,
        Colony = 2,
        Communism = 3,
        Confederacy = 4,
        Cooperative = 5,
        Corporate = 6,
        Democracy = 7,
        Dictatorship = 8,
        Feudal = 9,
        Imperial = 10,
        Patronage = 11,
        PrisonColony = 12,
        Theocracy = 13
    }

    public class StationDescription
    {
        public EnumStationWealth Wealth { get; private set; }
        public EnumPopulationSize Population { get; private set; }
        public EnumStationEconomy Economy { get; private set; }
        public EnumAllegance Allegance { get; private set; }
        public EnumGovernment Government { get; private set; }

        public StationDescription(EnumStationWealth wealth, EnumPopulationSize population, EnumStationEconomy economy, EnumAllegance allegance, EnumGovernment government)
        {
            Wealth = wealth;
            Population = population;
            Economy = economy;
            Allegance = allegance;
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
    }
}