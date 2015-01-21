using System;
using EliteTrader.EliteOcr.Enums;

namespace EliteTrader.EliteOcr.Data
{
    public static class CommodityItemHelper
    {
        public static EnumCommodityCategory GetCategory(EnumCommodityItemName item)
        {
            switch (item)
            {
                case EnumCommodityItemName.Explosives:
                case EnumCommodityItemName.HydrogenFuel:
                case EnumCommodityItemName.MineralOil:
                case EnumCommodityItemName.Pesticides:
                    return EnumCommodityCategory.Chemicals;
                case EnumCommodityItemName.Clothing:
                case EnumCommodityItemName.ConsumerTechnology:
                case EnumCommodityItemName.DomesticAppliances:
                    return EnumCommodityCategory.ConsumerItems;
                case EnumCommodityItemName.Algae:
                case EnumCommodityItemName.AnimalMeat:
                case EnumCommodityItemName.Coffee:
                case EnumCommodityItemName.Fish:
                case EnumCommodityItemName.FoodCartridges:
                case EnumCommodityItemName.FruitandVegetables:
                case EnumCommodityItemName.Grain:
                case EnumCommodityItemName.SyntheticMeat:
                case EnumCommodityItemName.Tea:
                    return EnumCommodityCategory.Foods;
                case EnumCommodityItemName.Polymers:
                case EnumCommodityItemName.Semiconductors:
                case EnumCommodityItemName.Superconductors:
                    return EnumCommodityCategory.IndustrialMaterials;
                case EnumCommodityItemName.Beer:
                case EnumCommodityItemName.Liquor:
                case EnumCommodityItemName.Narcotics:
                case EnumCommodityItemName.Tobacco:
                case EnumCommodityItemName.Wine:
                    return EnumCommodityCategory.LegalDrugs;
                case EnumCommodityItemName.AtmosphericProcessors:
                case EnumCommodityItemName.CropHarvesters:
                case EnumCommodityItemName.MarineEquipment:
                case EnumCommodityItemName.MicrobialFurnaces:
                case EnumCommodityItemName.MineralExtractors:
                case EnumCommodityItemName.PowerGenerators:
                case EnumCommodityItemName.WaterPurifiers:
                    return EnumCommodityCategory.Machinery;
                case EnumCommodityItemName.AgriMedicines:
                case EnumCommodityItemName.BasicMedicines:
                case EnumCommodityItemName.CombatStabilisers:
                case EnumCommodityItemName.PerformanceEnhancers:
                case EnumCommodityItemName.ProgenitorCells:
                    return EnumCommodityCategory.Medicines;
                case EnumCommodityItemName.Aluminium:
                case EnumCommodityItemName.Beryllium:
                case EnumCommodityItemName.Cobalt:
                case EnumCommodityItemName.Copper:
                case EnumCommodityItemName.Gallium:
                case EnumCommodityItemName.Gold:
                case EnumCommodityItemName.Indium:
                case EnumCommodityItemName.Lithium:
                case EnumCommodityItemName.Palladium:
                case EnumCommodityItemName.Platinum:
                case EnumCommodityItemName.Silver:
                case EnumCommodityItemName.Tantalum:
                case EnumCommodityItemName.Titanium:
                case EnumCommodityItemName.Uranium:
                    return EnumCommodityCategory.Metals;
                case EnumCommodityItemName.Bauxite:
                case EnumCommodityItemName.Bertrandite:
                case EnumCommodityItemName.Coltan:
                case EnumCommodityItemName.Gallite:
                case EnumCommodityItemName.Indite:
                case EnumCommodityItemName.Lepidolite:
                case EnumCommodityItemName.Rutile:
                case EnumCommodityItemName.Uraninite:
                    return EnumCommodityCategory.Minerals;
                case EnumCommodityItemName.ImperialSlaves:
                case EnumCommodityItemName.Slaves:
                    return EnumCommodityCategory.Slavery;
                case EnumCommodityItemName.AdvancedCatalysers:
                case EnumCommodityItemName.AnimalMonitors:
                case EnumCommodityItemName.AquaponicSystems:
                case EnumCommodityItemName.AutoFabricators:
                case EnumCommodityItemName.BioreducingLichen:
                case EnumCommodityItemName.ComputerComponents:
                case EnumCommodityItemName.HESuits:
                case EnumCommodityItemName.LandEnrichmentSystems:
                case EnumCommodityItemName.ResonatingSeparators:
                case EnumCommodityItemName.Robotics:
                    return EnumCommodityCategory.Technology;
                case EnumCommodityItemName.Leather:
                case EnumCommodityItemName.NaturalFabrics:
                case EnumCommodityItemName.SyntheticFabrics:
                    return EnumCommodityCategory.Textiles;
                case EnumCommodityItemName.Biowaste:
                case EnumCommodityItemName.ChemicalWaste:
                case EnumCommodityItemName.Scrap:
                    return EnumCommodityCategory.Waste;
                case EnumCommodityItemName.BattleWeapons:
                case EnumCommodityItemName.NonLethalWpns:
                case EnumCommodityItemName.PersonalWeapons:
                case EnumCommodityItemName.ReactiveArmour:
                    return EnumCommodityCategory.Weapons;
                default:
                    throw new ArgumentException(string.Format("Unknown commodity item name ({0})", item));
            }

        }
    }
}
