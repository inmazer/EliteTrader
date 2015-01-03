using EliteTrader.EliteOcr.Enums;
using EliteTrader.EliteOcr.Interfaces;

namespace EliteTrader.EliteOcr.Data
{
    public class CommodityItemNameMatcher : ICommodityNameMatcher
    {
        public CommodityName FromString(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new CommodityName(EnumCommodityItemName.Unknown, null);
            }
            name = name.ToUpper();

            EnumCommodityItemName enumName;
            if (DefaultCommodityItems.ItemNames.TryGetValue(name, out enumName))
            {
                return new CommodityName(enumName, null);
            }

            if (DetfaultRareCommodities.RareNames.Contains(name))
            {
                return new CommodityName(EnumCommodityItemName.Rare, name);
            }

            return new CommodityName(EnumCommodityItemName.Unknown, null);
        }
    }
}
