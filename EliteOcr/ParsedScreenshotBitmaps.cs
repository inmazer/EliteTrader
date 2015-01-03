using System;
using System.Collections.Generic;
using System.Drawing;

namespace EliteTrader.EliteOcr
{
    public class ParsedScreenshotBitmaps : IDisposable
    {
        public Bitmap Name { get; private set; }
        public Bitmap Description { get; private set; }
        public List<CommodityItemBitmaps> ItemBitmapsList { get; private set; }

        public ParsedScreenshotBitmaps(Bitmap name, Bitmap description, List<CommodityItemBitmaps> itemBitmapsList)
        {
            Name = name;
            Description = description;
            ItemBitmapsList = itemBitmapsList;
        }

        public void Dispose()
        {
            if (Name != null)
            {
                Name.Dispose();
                Name = null;
            }
            if (Description != null)
            {
                Description.Dispose();
                Description = null;
            }
            if (ItemBitmapsList != null)
            {
                foreach (CommodityItemBitmaps items in ItemBitmapsList)
                {
                    items.Dispose();
                }
                ItemBitmapsList = null;
            }

        }
    }
}
