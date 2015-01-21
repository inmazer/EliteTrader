using System.Collections.Generic;
using EliteTrader.EliteOcr.Enums;

namespace EliteTrader.EliteOcr.Interfaces
{
    public interface ICommodityNameRepository
    {
        List<string> GetAll();
        string Get(EnumCommodityItemName name);
    }
}