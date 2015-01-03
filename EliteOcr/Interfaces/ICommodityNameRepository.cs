using System.Collections.Generic;

namespace EliteTrader.EliteOcr.Interfaces
{
    public interface ICommodityNameRepository
    {
        List<string> GetAllCommodityNames();
    }
}