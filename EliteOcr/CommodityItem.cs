using EliteTrader.EliteOcr.Enums;

namespace EliteTrader.EliteOcr
{
    public class CommodityItem
    {
        public EnumCommodityItemName Name { get; private set; }
        public string RareName { get; private set; }
        public int? Sell { get; private set; }
        public int? Buy { get; private set; }
        public int? Demand { get; private set; }
        public EnumSupplyStatus DemandStatus { get; private set; }
        public int? Supply { get; private set; }
        public EnumSupplyStatus SupplyStatus { get; private set; }
        public int GalacticAverage { get; private set; }

        public CommodityItem(EnumCommodityItemName name, string rareName, int? sell, int? buy, int? demand, EnumSupplyStatus demandStatus, int? supply, EnumSupplyStatus supplyStatus, int galacticAverage)
        {
            Name = name;
            RareName = rareName;
            Sell = sell;
            Buy = buy;
            Demand = demand;
            DemandStatus = demandStatus;
            Supply = supply;
            SupplyStatus = supplyStatus;
            GalacticAverage = galacticAverage;
        }

        public override string ToString()
        {
            return string.Format("{0,-30} | {1,-7} | {2,-7} | {3,-12} | {4,-12} | {5,-10}",
                Name == EnumCommodityItemName.Rare
                    ? RareName
                    : Name.ToString(), Sell, Buy,
                Demand.HasValue ? string.Format("{0} {1}", Demand.Value, DemandStatus) : "",
                Supply.HasValue ? string.Format("{0} {1}", Supply.Value, SupplyStatus) : "", GalacticAverage);
        }
    }
}