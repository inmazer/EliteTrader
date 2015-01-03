using System;
using System.IO;

namespace EliteTrader.EliteOcr
{
    public class CommodityItemBitmaps : IDisposable
    {
        public PartialItemBitmap Name { get; private set; }
        public PartialItemBitmap Sell { get; private set; }
        public PartialItemBitmap Buy { get; private set; }
        public PartialItemBitmap Demand { get; private set; }
        public PartialItemBitmap Supply { get; private set; }
        public PartialItemBitmap GalacticAverage { get; private set; }

        public CommodityItemBitmaps(PartialItemBitmap name, PartialItemBitmap sell, PartialItemBitmap buy, PartialItemBitmap supply, PartialItemBitmap demand, PartialItemBitmap galacticAverage)
        {
            Name = name;
            Sell = sell;
            Buy = buy;
            Supply = supply;
            Demand = demand;
            GalacticAverage = galacticAverage;
        }

        public void Save(string path, int index)
        {
            Name.Save(string.Format(Path.Combine(path, string.Format("Name{0}.bmp", index))));
            Sell.Save(string.Format(Path.Combine(path, string.Format("Sell{0}.bmp", index))));
            Buy.Save(string.Format(Path.Combine(path, string.Format("Buy{0}.bmp", index))));
            Demand.Save(string.Format(Path.Combine(path, string.Format("Demand{0}.bmp", index))));
            Supply.Save(string.Format(Path.Combine(path, string.Format("Supply{0}.bmp", index))));
            GalacticAverage.Save(string.Format(Path.Combine(path, string.Format("GalacticAverage{0}.bmp", index))));
        }

        public void Dispose()
        {
            if (Name != null)
            {
                Name.Dispose();
                Name = null;
            }
            if (Sell != null)
            {
                Sell.Dispose();
                Sell = null;
            }
            if (Buy != null)
            {
                Buy.Dispose();
                Buy = null;
            }
            if (Demand != null)
            {
                Demand.Dispose();
                Demand = null;
            }
            if (Supply != null)
            {
                Supply.Dispose();
                Supply = null;
            }
            if (GalacticAverage != null)
            {
                GalacticAverage.Dispose();
                GalacticAverage = null;
            }
        }
    }
}