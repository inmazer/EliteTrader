using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThruddClient;

namespace EliteOcrTests
{
    [TestClass]
    public class TestCreateStaticData : TestScreenshotBase
    {
        //[TestMethod]
        //public void PrintStaticData()
        //{
        //    Client client = new Client();

        //    GetSelectListsResult lists = client.GetSelectLists();

        //    PrintEnum("EnumAllegiance", ToTuples(lists.Allegiances));
        //    PrintEnum("EnumStationEconomy", ToTuples(lists.Economies));
        //    PrintEnum("EnumGovernment", ToTuples(lists.Governments));
        //    PrintEnum("EnumStationType", ToTuples(lists.StationTypes));

        //    const string categoryEnumName = "EnumCommodityCategory";
        //    const string commodityItemsEnumName = "EnumCommodityItemName";
        //    PrintEnum(categoryEnumName, ToTuples(lists.CommodityCategories));

        //    PrintEnum(commodityItemsEnumName, ToTuples(lists.CommodityCategories.SelectMany(a => a.Commodities).ToList()));

        //    PrintCategorySwitch(categoryEnumName, commodityItemsEnumName, lists.CommodityCategories);
        //    client.Logout();
        //}

        private List<KeyValuePair<string, int>> ToTuples(List<CommodityItem> items)
        {
            List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();

            list.Add(new KeyValuePair<string, int>("Unknown", 0));

            foreach (CommodityItem item in items.OrderBy(a => a.Value))
            {
                list.Add(new KeyValuePair<string, int>(item.Text, item.Value));
            }

            list.Add(new KeyValuePair<string, int>("Rare", 200));

            return list;
        }

        private List<KeyValuePair<string, int>> ToTuples(List<CommodityCategory> items)
        {
            List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();

            list.Add(new KeyValuePair<string, int>("Unknown", 0));

            foreach (CommodityCategory item in items.OrderBy(a => a.Value))
            {
                list.Add(new KeyValuePair<string, int>(item.Text, item.Value));
            }

            return list;
        }

        private List<KeyValuePair<string, int>> ToTuples(List<SelectedListItem> items)
        {
            List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();

            list.Add(new KeyValuePair<string, int>("Unknown", 0));

            foreach (SelectedListItem item in items.OrderBy(a => int.Parse(a.Value)))
            {
                list.Add(new KeyValuePair<string, int>(item.Text, int.Parse(item.Value)));
            }

            return list;
        }

        private void PrintEnum(string enumName, List<KeyValuePair<string, int>> tuples)
        {
            Console.WriteLine(string.Format("public enum {0}", enumName));
            Console.WriteLine("{");
            foreach (KeyValuePair<string, int> tuple in tuples)
            {
                Console.WriteLine("    {0} = {1},", GetEnumName(tuple.Key), tuple.Value);
            }
            Console.WriteLine("}");
            Console.WriteLine();
        }

        private string GetEnumName(string text)
        {
            return text.Replace(" ", "").Replace("-", "").Replace(".", "").Replace("'", "").Replace("\r\n", "");
        }

        private void PrintCategorySwitch(string categoryEnumName, string commodityItemsEnumName, List<CommodityCategory> categories)
        {
            Console.WriteLine("switch(item)");
            Console.WriteLine("{");
            foreach (CommodityCategory category in categories)
            {
                foreach (CommodityItem item in category.Commodities)
                {
                    Console.WriteLine("    case {0}.{1}:", commodityItemsEnumName, GetEnumName(item.Text));
                }
                Console.WriteLine("        return {0}.{1};", categoryEnumName, GetEnumName(category.Text));
            }
            Console.WriteLine("    default:");
            Console.WriteLine("        throw new ArgumentException(string.Format(\"Unknown commodity item name ({0})\", item));");
            Console.WriteLine("}");
            Console.WriteLine();
        }

    }
}
