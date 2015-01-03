using EliteTrader.EliteOcr.Enums;

namespace EliteTrader.EliteOcr
{
    public class CommodityName
    {
        public EnumCommodityItemName Name { get; private set; }
        public string RareName { get; private set; }

        public CommodityName(EnumCommodityItemName name, string rareName)
        {
            Name = name;
            RareName = rareName;
        }

        public override string ToString()
        {
            return Name == EnumCommodityItemName.Rare ? string.Format("{0} (Rare)", RareName) : Name.ToString();
        }
    }
}