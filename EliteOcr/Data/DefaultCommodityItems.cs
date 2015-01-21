using System.Collections.Generic;
using System.Linq;
using EliteTrader.EliteOcr.Enums;

namespace EliteTrader.EliteOcr.Data
{
    public static class DefaultCommodityItems
    {
        public static readonly Dictionary<string, EnumCommodityItemName> ItemNames = new Dictionary<string, EnumCommodityItemName>();
        public static readonly Dictionary<EnumCommodityItemName, string> ItemNamesReverse = new Dictionary<EnumCommodityItemName, string>();

        static DefaultCommodityItems()
        {
            ItemNames.Add("EXPLOSIVES", EnumCommodityItemName.Explosives);
            ItemNames.Add("HYDROGEN FUEL", EnumCommodityItemName.HydrogenFuel);
            ItemNames.Add("MINERAL OIL", EnumCommodityItemName.MineralOil);
            ItemNames.Add("PESTICIDES", EnumCommodityItemName.Pesticides);
            ItemNames.Add("CLOTHING", EnumCommodityItemName.Clothing);
            ItemNames.Add("CONSUMER TECHNOLOGY", EnumCommodityItemName.ConsumerTechnology);
            ItemNames.Add("DOMESTIC APPLIANCES", EnumCommodityItemName.DomesticAppliances);
            ItemNames.Add("ALGAE", EnumCommodityItemName.Algae);
            ItemNames.Add("ANIMAL MEAT", EnumCommodityItemName.AnimalMeat);
            ItemNames.Add("COFFEE", EnumCommodityItemName.Coffee);
            ItemNames.Add("FISH", EnumCommodityItemName.Fish);
            ItemNames.Add("FOOD CARTRIDGES", EnumCommodityItemName.FoodCartridges);
            ItemNames.Add("FRUIT AND VEGETABLES", EnumCommodityItemName.FruitandVegetables);
            ItemNames.Add("GRAIN", EnumCommodityItemName.Grain);
            ItemNames.Add("SYNTHETIC MEAT", EnumCommodityItemName.SyntheticMeat);
            ItemNames.Add("TEA", EnumCommodityItemName.Tea);
            ItemNames.Add("POLYMERS", EnumCommodityItemName.Polymers);
            ItemNames.Add("SEMICONDUCTORS", EnumCommodityItemName.Semiconductors);
            ItemNames.Add("SUPERCONDUCTORS", EnumCommodityItemName.Superconductors);
            ItemNames.Add("BEER", EnumCommodityItemName.Beer);
            ItemNames.Add("LIQUOR", EnumCommodityItemName.Liquor);
            ItemNames.Add("NARCOTICS", EnumCommodityItemName.Narcotics);
            ItemNames.Add("TOBACCO", EnumCommodityItemName.Tobacco);
            ItemNames.Add("WINE", EnumCommodityItemName.Wine);
            ItemNames.Add("ATMOSPHERIC PROCESSORS", EnumCommodityItemName.AtmosphericProcessors);
            ItemNames.Add("CROP HARVESTERS", EnumCommodityItemName.CropHarvesters);
            ItemNames.Add("MARINE EQUIPMENT", EnumCommodityItemName.MarineEquipment);
            ItemNames.Add("MICROBIAL FURNACES", EnumCommodityItemName.MicrobialFurnaces);
            ItemNames.Add("MINERAL EXTRACTORS", EnumCommodityItemName.MineralExtractors);
            ItemNames.Add("POWER GENERATORS", EnumCommodityItemName.PowerGenerators);
            ItemNames.Add("WATER PURIFIERS", EnumCommodityItemName.WaterPurifiers);
            ItemNames.Add("AGRI-MEDICINES", EnumCommodityItemName.AgriMedicines);
            ItemNames.Add("BASIC MEDICINES", EnumCommodityItemName.BasicMedicines);
            ItemNames.Add("COMBAT STABILISERS", EnumCommodityItemName.CombatStabilisers);
            ItemNames.Add("PERFORMANCE ENHANCERS", EnumCommodityItemName.PerformanceEnhancers);
            ItemNames.Add("PROGENITOR CELLS", EnumCommodityItemName.ProgenitorCells);
            ItemNames.Add("ALUMINIUM", EnumCommodityItemName.Aluminium);
            ItemNames.Add("BERYLLIUM", EnumCommodityItemName.Beryllium);
            ItemNames.Add("COBALT", EnumCommodityItemName.Cobalt);
            ItemNames.Add("COPPER", EnumCommodityItemName.Copper);
            ItemNames.Add("GALLIUM", EnumCommodityItemName.Gallium);
            ItemNames.Add("GOLD", EnumCommodityItemName.Gold);
            ItemNames.Add("INDIUM", EnumCommodityItemName.Indium);
            ItemNames.Add("LITHIUM", EnumCommodityItemName.Lithium);
            ItemNames.Add("PALLADIUM", EnumCommodityItemName.Palladium);
            ItemNames.Add("PLATINUM", EnumCommodityItemName.Platinum);
            ItemNames.Add("SILVER", EnumCommodityItemName.Silver);
            ItemNames.Add("TANTALUM", EnumCommodityItemName.Tantalum);
            ItemNames.Add("TITANIUM", EnumCommodityItemName.Titanium);
            ItemNames.Add("URANIUM", EnumCommodityItemName.Uranium);
            ItemNames.Add("BAUXITE", EnumCommodityItemName.Bauxite);
            ItemNames.Add("BERTRANDITE", EnumCommodityItemName.Bertrandite);
            ItemNames.Add("COLTAN", EnumCommodityItemName.Coltan);
            ItemNames.Add("GALLITE", EnumCommodityItemName.Gallite);
            ItemNames.Add("INDITE", EnumCommodityItemName.Indite);
            ItemNames.Add("LEPIDOLITE", EnumCommodityItemName.Lepidolite);
            ItemNames.Add("RUTILE", EnumCommodityItemName.Rutile);
            ItemNames.Add("URANINITE", EnumCommodityItemName.Uraninite);
            ItemNames.Add("IMPERIAL SLAVES", EnumCommodityItemName.ImperialSlaves);
            ItemNames.Add("SLAVES", EnumCommodityItemName.Slaves);
            ItemNames.Add("ADVANCED CATALYSERS", EnumCommodityItemName.AdvancedCatalysers);
            ItemNames.Add("ANIMAL MONITORS", EnumCommodityItemName.AnimalMonitors);
            ItemNames.Add("AQUAPONIC SYSTEMS", EnumCommodityItemName.AquaponicSystems);
            ItemNames.Add("AUTO-FABRICATORS", EnumCommodityItemName.AutoFabricators);
            ItemNames.Add("BIOREDUCING LICHEN", EnumCommodityItemName.BioreducingLichen);
            ItemNames.Add("COMPUTER COMPONENTS", EnumCommodityItemName.ComputerComponents);
            ItemNames.Add("H.E. SUITS", EnumCommodityItemName.HESuits);
            ItemNames.Add("LAND ENRICHMENT SYSTEMS", EnumCommodityItemName.LandEnrichmentSystems);
            ItemNames.Add("RESONATING SEPARATORS", EnumCommodityItemName.ResonatingSeparators);
            ItemNames.Add("ROBOTICS", EnumCommodityItemName.Robotics);
            ItemNames.Add("LEATHER", EnumCommodityItemName.Leather);
            ItemNames.Add("NATURAL FABRICS", EnumCommodityItemName.NaturalFabrics);
            ItemNames.Add("SYNTHETIC FABRICS", EnumCommodityItemName.SyntheticFabrics);
            ItemNames.Add("BIOWASTE", EnumCommodityItemName.Biowaste);
            ItemNames.Add("CHEMICAL WASTE", EnumCommodityItemName.ChemicalWaste);
            ItemNames.Add("SCRAP", EnumCommodityItemName.Scrap);
            ItemNames.Add("BATTLE WEAPONS", EnumCommodityItemName.BattleWeapons);
            ItemNames.Add("NON-LETHAL WEAPONS", EnumCommodityItemName.NonLethalWpns);
            ItemNames.Add("PERSONAL WEAPONS", EnumCommodityItemName.PersonalWeapons);
            ItemNames.Add("REACTIVE ARMOUR", EnumCommodityItemName.ReactiveArmour);

            ItemNamesReverse = ItemNames.ToDictionary(a => a.Value, a => a.Key);
        }
    }
}
