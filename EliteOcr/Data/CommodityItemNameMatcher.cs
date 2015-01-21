using System;
using System.Collections.Generic;
using EliteTrader.EliteOcr.Enums;
using EliteTrader.EliteOcr.Interfaces;

namespace EliteTrader.EliteOcr.Data
{
    public class CommodityItemNameMatcher : ICommodityNameMatcher, ICommodityNameRepository
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

        public List<string> GetAll()
        {
            List<string> result = new List<string>();

            result.AddRange(DefaultCommodityItems.ItemNames.Keys);
            result.AddRange(DetfaultRareCommodities.RareNames);

            return result;
        }

        public string Get(EnumCommodityItemName name)
        {
            string str;
            if (!DefaultCommodityItems.ItemNamesReverse.TryGetValue(name, out str))
            {
                throw new Exception(string.Format("Unable to find commodity name for enum value ({0})", name));
            }
            return str;
        }
    }
}
