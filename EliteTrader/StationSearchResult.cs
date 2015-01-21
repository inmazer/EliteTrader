using System.Collections.Generic;
using ThruddClient;

namespace EliteTrader
{
    public class StationSearchResult
    {
        public List<StationCommoditiesData> StationCommodities { get; private set; }
        public AdminSearchResultItem StationInfo { get; private set; }

        public StationSearchResult(List<StationCommoditiesData> stationCommodities, AdminSearchResultItem stationInfo)
        {
            StationCommodities = stationCommodities;
            StationInfo = stationInfo;
        }
    }
}