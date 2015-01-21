using System.Collections.Generic;
using EliteTrader.EliteOcr;
using EliteTrader.EliteOcr.Enums;

namespace ThruddClient
{
    public class AdminSearchResultItem
    {
        public int StationId { get; set; }
        public string Station { get; set; }
        public string System { get; set; }
        public int SystemId { get; set; }
    }

    public class SystemData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Economy { get; set; }
        public int GovernmentId { get; set; }
        public string GovernmentName { get; set; }
        public int AllegianceId { get; set; }
        public string AllegianceName { get; set; }
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal Z { get; set; }
    }

    public class SystemStationsData
    {
        public int SystemId { get; set; }
        public int MarketStationId { get; set; }
        public List<StationData> Stations { get; set; } 
    }

    public class StationData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SystemId { get; set; }
        public bool HasBlackMarket { get; set; }
        public bool HasMarket { get; set; }
        public bool HasOutfitting { get; set; }
        public bool HasShipyard { get; set; }
        public bool HasRepairs { get; set; }
        public int AllegianceId { get; set; }
        public string Allegiance { get; set; }
        public int EconomyId { get; set; }
        public int? SecondaryEconomyId { get; set; }
        public string Economy { get; set; }
        public string SecondaryEconomy { get; set; }
        public int GovernmentId { get; set; }
        public string Government { get; set; }
        public int DistanceFromJumpIn { get; set; }
        public int StationTypeId { get; set; }
        public string StationTypeName { get; set; }
        public int CopyMarketFromStationId { get; set; }

        public EnumAllegiance AllegianceEnum { get { return (EnumAllegiance) AllegianceId; } }
        public EnumStationEconomy EconomyEnum { get { return (EnumStationEconomy)EconomyId; } }
        public EnumStationEconomy? SecondaryEconomyEnum { get { return SecondaryEconomyId != null ? (EnumStationEconomy?)SecondaryEconomyId : null; } }
        public EnumGovernment GovernmentEnum { get { return (EnumGovernment)GovernmentId; } }
        public EnumStationType StationTypeEnum { get { return (EnumStationType)StationTypeId; } }
    }

    public class StationCommoditiesResult
    {
        public List<StationCommoditiesData> StationCommodities { get; set; } 
    }

    public class StationCommoditiesData
    {
        public string TimeSince { get; set; }
        public string CommodityName { get; set; }
        public string StationName { get; set; }
        public int Id { get; set; } //Use this Id to update the commodity prices (StationCommodityId)
        public int StationId { get; set; }
        public string Station { get; set; }
        public int CommodityId { get; set; }
        public string Commodity { get; set; }
        public int Buy { get; set; }
        public int Sell { get; set; }
        public string LastUpdate { get; set; }
        public string UpdatedBy { get; set; }
        public int Version { get; set; }

        public EnumCommodityItemName CommodityEnum { get { return (EnumCommodityItemName)CommodityId; } }
    }

    public enum EnumCommodityAction
    {
        Sell = 1,
        Buy = 2
    }

    public class UpdateCommodityResponse
    {
        public int StationCommodityId { get; set; }
        public string Action { get; set; }
        public int Value { get; set; }
        public string LastUpdate { get; set; }
        public string UpdatedBy { get; set; }

        public RepResultData RepResult { get; set; }
    }

    public class RepResultData
    {
        public string CommanderName { get; set; }
        public int Reputation { get; set; }
        public int ReputationNeeded { get; set; }
        public string Title { get; set; }
        public bool RankUp { get; set; }
        public string Badge { get; set; }
    }

    public enum EnumPadSize
    {
        Small,
        Medium,
        Large
    }

    public class FindTradesInfo
    {
        public string SearchRange { get; private set; }
        public string SystemName { get; private set; }
        public string MinProfitPerTon { get; private set; }
        public EnumPadSize PadSize { get; private set; }

        public FindTradesInfo(int searchRange, string systemName, int minProfitPerTon, EnumPadSize padSize)
        {
            SearchRange = searchRange.ToString();
            SystemName = systemName;
            MinProfitPerTon = minProfitPerTon.ToString();
            PadSize = padSize;
        }
    }

    public class GetSelectListsResult
    {
        public List<SelectedListItem> Allegiances { get; set; }
        public List<SelectedListItem> Economies { get; set; }
        public List<SelectedListItem> Governments { get; set; } 
        //Systems
        public List<SelectedListItem> StationTypes { get; set; }
        public List<CommodityCategory> CommodityCategories { get; set; } 

    }

    public class SelectedListItem
    {
        public bool Disables { get; set; }
        public string Group { get; set; }
        public bool Selected { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
    }

    public class CommodityCategory
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public List<CommodityItem> Commodities { get; set; }
    }

    public class CommodityItem
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }

    /*
     * POST http://www.elitetradingtool.co.uk/Admin/AddEditStationCommodity HTTP/1.1
Host: www.elitetradingtool.co.uk
Connection: keep-alive
Content-Length: 83

Origin: http://www.elitetradingtool.co.uk
X-Requested-With: XMLHttpRequest
User-Agent: Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36
Content-Type: application/json
Referer: http://www.elitetradingtool.co.uk/Admin/
Accept-Encoding: gzip, deflate
Accept-Language: en-US,en;q=0.8,da;q=0.6,nb;q=0.4,sv;q=0.2
Cookie: __RequestVerificationToken=_PRcL13C7hVPAT0CWWimiF6EzWOo9BB_yhXcrOBvy0CU3gwcF7vAMs7YpvGqk-Dst3Us_G-5K7mLqxO5kkn-AlMNoZBoSvWYFzEj9u-l7y41; .AspNet.ApplicationCookie=F8isL-y4eArX8KXxWcFgt0nTJeBdRSAFctFiKYvfa2MMsz-5vzJp5I1k-LOtfFsE-pjX1nhT8yRNjBLNdh4R9EU0ZK0eNoEgK02j3gbMf_yT8NWDIyU0kXcz82-Bub88bJX0m3EO1hacwFX0vPH-RT1vWMarXHRmkxLiSnSIEWZ0t-nSrMsPb9de3_vd4NE1Gg-oZRRVL6Eu1VrvYA9SOMeEJ2YMRODSHKLxwEaGZx6BxS5EdIq1xcR9VxAwnNeOMmdKOTlBjRuZoeTFL8ACPOe0raBlhxLbBDYY2OcmVPR2hmFuj5SdNr0gxFWHrHAKlyanSK-W1RMyt8EDV80HOm6kP8E-vSztxt70IOqQ1JKz6GnjD0puTjkQdGh586TKkoFNYxAREjBmrsIXWrh8XMyC_dEpzhbtPifSGjnuNArf0kYPd7f7mrxQMmo4Wl07206lN-JODTpa3CstB9RCE8nmeZIzERrOvUHiKHETSZWZDGsSdg7cS9svAqYXs9U3UtKsWJxLjogWWsU856m214yn5FFbAnf4FfEUyebyhYhAy7TBL3rzIQfbiuj8CJAQuC19U2dSdFbLFlMslTrIFJpVHEYoilj_8vYmBh8Iawk; _gat=1; _ga=GA1.3.139030831.1419415951

{"Id":0,"StationId":1078,"CommodityId":"2","Buy":"0","Sell":"366","CategoryId":"1"}
     */

    /*
     HTTP/1.1 302 Found
Date: Fri, 09 Jan 2015 04:21:27 GMT
Server: Microsoft-IIS/8.5
Cache-Control: private, s-maxage=0
Content-Type: text/html; charset=utf-8
Location: /Admin/StationCommodities/1078?categoryId=1
X-AspNetMvc-Version: 5.2
X-AspNet-Version: 4.0.30319
X-Powered-By: ASP.NET
Content-Length: 160
Connection: close

<html><head><title>Object moved</title></head><body>
<h2>Object moved to <a href="/Admin/StationCommodities/1078?categoryId=1">here</a>.</h2>
</body></html>
*/

    /*
     * POST http://www.elitetradingtool.co.uk/Admin/DeleteStationCommodity HTTP/1.1
Host: www.elitetradingtool.co.uk
Connection: keep-alive
Content-Length: 30
Origin: http://www.elitetradingtool.co.uk
X-Requested-With: XMLHttpRequest
User-Agent: Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36
Content-Type: application/json
Referer: http://www.elitetradingtool.co.uk/Admin/
Accept-Encoding: gzip, deflate
Accept-Language: en-US,en;q=0.8,da;q=0.6,nb;q=0.4,sv;q=0.2
Cookie: __RequestVerificationToken=_PRcL13C7hVPAT0CWWimiF6EzWOo9BB_yhXcrOBvy0CU3gwcF7vAMs7YpvGqk-Dst3Us_G-5K7mLqxO5kkn-AlMNoZBoSvWYFzEj9u-l7y41; .AspNet.ApplicationCookie=F8isL-y4eArX8KXxWcFgt0nTJeBdRSAFctFiKYvfa2MMsz-5vzJp5I1k-LOtfFsE-pjX1nhT8yRNjBLNdh4R9EU0ZK0eNoEgK02j3gbMf_yT8NWDIyU0kXcz82-Bub88bJX0m3EO1hacwFX0vPH-RT1vWMarXHRmkxLiSnSIEWZ0t-nSrMsPb9de3_vd4NE1Gg-oZRRVL6Eu1VrvYA9SOMeEJ2YMRODSHKLxwEaGZx6BxS5EdIq1xcR9VxAwnNeOMmdKOTlBjRuZoeTFL8ACPOe0raBlhxLbBDYY2OcmVPR2hmFuj5SdNr0gxFWHrHAKlyanSK-W1RMyt8EDV80HOm6kP8E-vSztxt70IOqQ1JKz6GnjD0puTjkQdGh586TKkoFNYxAREjBmrsIXWrh8XMyC_dEpzhbtPifSGjnuNArf0kYPd7f7mrxQMmo4Wl07206lN-JODTpa3CstB9RCE8nmeZIzERrOvUHiKHETSZWZDGsSdg7cS9svAqYXs9U3UtKsWJxLjogWWsU856m214yn5FFbAnf4FfEUyebyhYhAy7TBL3rzIQfbiuj8CJAQuC19U2dSdFbLFlMslTrIFJpVHEYoilj_8vYmBh8Iawk; _gat=1; _ga=GA1.3.139030831.1419415951

{"id":364616,"categoryId":"1"}
    */

    /*
     * HTTP/1.1 302 Found
Date: Fri, 09 Jan 2015 04:23:00 GMT
Server: Microsoft-IIS/8.5
Cache-Control: private, s-maxage=0
Content-Type: text/html; charset=utf-8
Location: /Admin/StationCommodities/1078?categoryId=1
X-AspNetMvc-Version: 5.2
X-AspNet-Version: 4.0.30319
X-Powered-By: ASP.NET
Content-Length: 160
Connection: close

<html><head><title>Object moved</title></head><body>
<h2>Object moved to <a href="/Admin/StationCommodities/1078?categoryId=1">here</a>.</h2>
</body></html>
*/
}