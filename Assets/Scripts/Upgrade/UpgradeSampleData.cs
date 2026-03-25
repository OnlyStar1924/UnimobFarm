using System.Collections.Generic;
using UnityEngine;

public static class UpgradeSampleData
{
    public static List<UpgradeItemData> Create(
        Sprite wheatIcon,
        Sprite woodIcon,
        Sprite clayIcon,
        Sprite steelIcon,
        Sprite customerIcon,
        Sprite globalProfitIcon)
    {
        List<UpgradeItemData> items = new List<UpgradeItemData>();

        // WHEAT
        items.Add(new UpgradeItemData
        {
            id = "wheat_profit_1",
            title = "Wheat Lv1",
            desc = "Wheat x2 Profit",
            icon = wheatIcon,
            cost = 1000,
            effectType = UpgradeEffectType.ConstructionProfit,
            targetConstructionType = ConstructionType.Wheat,
            profitMultiplier = 2f,
            unlockAfterId = "",
            purchased = false
        });

        items.Add(new UpgradeItemData
        {
            id = "wheat_profit_2",
            title = "Wheat Lv2",
            desc = "Wheat x2 Profit",
            icon = wheatIcon,
            cost = 2000,
            effectType = UpgradeEffectType.ConstructionProfit,
            targetConstructionType = ConstructionType.Wheat,
            profitMultiplier = 2f,
            unlockAfterId = "wheat_profit_1",
            purchased = false
        });

        items.Add(new UpgradeItemData
        {
            id = "wheat_profit_3",
            title = "Wheat Lv3",
            desc = "Wheat x2 Profit",
            icon = wheatIcon,
            cost = 4000,
            effectType = UpgradeEffectType.ConstructionProfit,
            targetConstructionType = ConstructionType.Wheat,
            profitMultiplier = 2f,
            unlockAfterId = "wheat_profit_2",
            purchased = false
        });

        // WOOD
        items.Add(new UpgradeItemData
        {
            id = "wood_profit_1",
            title = "Wood Lv1",
            desc = "Wood x2 Profit",
            icon = woodIcon,
            cost = 5000,
            effectType = UpgradeEffectType.ConstructionProfit,
            targetConstructionType = ConstructionType.Wood,
            profitMultiplier = 2f,
            unlockAfterId = "",
            purchased = false
        });

        items.Add(new UpgradeItemData
        {
            id = "wood_profit_2",
            title = "Wood Lv2",
            desc = "Wood x2 Profit",
            icon = woodIcon,
            cost = 10000,
            effectType = UpgradeEffectType.ConstructionProfit,
            targetConstructionType = ConstructionType.Wood,
            profitMultiplier = 2f,
            unlockAfterId = "wood_profit_1",
            purchased = false
        });

        items.Add(new UpgradeItemData
        {
            id = "wood_profit_3",
            title = "Wood Lv3",
            desc = "Wood x2 Profit",
            icon = woodIcon,
            cost = 20000,
            effectType = UpgradeEffectType.ConstructionProfit,
            targetConstructionType = ConstructionType.Wood,
            profitMultiplier = 2f,
            unlockAfterId = "wood_profit_2",
            purchased = false
        });

        // CLAY
        items.Add(new UpgradeItemData
        {
            id = "clay_profit_1",
            title = "Clay Lv1",
            desc = "Clay x2 Profit",
            icon = clayIcon,
            cost = 100000,
            effectType = UpgradeEffectType.ConstructionProfit,
            targetConstructionType = ConstructionType.Clay,
            profitMultiplier = 2f,
            unlockAfterId = "",
            purchased = false
        });

        items.Add(new UpgradeItemData
        {
            id = "clay_profit_2",
            title = "Clay Lv2",
            desc = "Clay x2 Profit",
            icon = clayIcon,
            cost = 200000,
            effectType = UpgradeEffectType.ConstructionProfit,
            targetConstructionType = ConstructionType.Clay,
            profitMultiplier = 2f,
            unlockAfterId = "clay_profit_1",
            purchased = false
        });

        items.Add(new UpgradeItemData
        {
            id = "clay_profit_3",
            title = "Clay Lv3",
            desc = "Clay x2 Profit",
            icon = clayIcon,
            cost = 400000,
            effectType = UpgradeEffectType.ConstructionProfit,
            targetConstructionType = ConstructionType.Clay,
            profitMultiplier = 2f,
            unlockAfterId = "clay_profit_2",
            purchased = false
        });

        // STEEL
        items.Add(new UpgradeItemData
        {
            id = "steel_profit_1",
            title = "Steel Lv1",
            desc = "Steel x2 Profit",
            icon = steelIcon,
            cost = 1000000,
            effectType = UpgradeEffectType.ConstructionProfit,
            targetConstructionType = ConstructionType.Steel,
            profitMultiplier = 2f,
            unlockAfterId = "",
            purchased = false
        });

        items.Add(new UpgradeItemData
        {
            id = "steel_profit_2",
            title = "Steel Lv2",
            desc = "Steel x2 Profit",
            icon = steelIcon,
            cost = 2000000,
            effectType = UpgradeEffectType.ConstructionProfit,
            targetConstructionType = ConstructionType.Steel,
            profitMultiplier = 2f,
            unlockAfterId = "steel_profit_1",
            purchased = false
        });

        items.Add(new UpgradeItemData
        {
            id = "steel_profit_3",
            title = "Steel Lv3",
            desc = "Steel x2 Profit",
            icon = steelIcon,
            cost = 4000000,
            effectType = UpgradeEffectType.ConstructionProfit,
            targetConstructionType = ConstructionType.Steel,
            profitMultiplier = 2f,
            unlockAfterId = "steel_profit_2",
            purchased = false
        });

        // GLOBAL PROFIT
        items.Add(new UpgradeItemData
        {
            id = "global_profit_1",
            title = "Global Profit Lv1",
            desc = "All Construction x2 Profit",
            icon = globalProfitIcon,
            cost = 100000,
            effectType = UpgradeEffectType.GlobalProfit,
            profitMultiplier = 2f,
            unlockAfterId = "",
            purchased = false
        });

        items.Add(new UpgradeItemData
        {
            id = "global_profit_2",
            title = "Global Profit Lv2",
            desc = "All Construction x2 Profit",
            icon = globalProfitIcon,
            cost = 200000,
            effectType = UpgradeEffectType.GlobalProfit,
            profitMultiplier = 2f,
            unlockAfterId = "global_profit_1",
            purchased = false
        });

        items.Add(new UpgradeItemData
        {
            id = "global_profit_3",
            title = "Global Profit Lv3",
            desc = "All Construction x2 Profit",
            icon = globalProfitIcon,
            cost = 400000,
            effectType = UpgradeEffectType.GlobalProfit,
            profitMultiplier = 2f,
            unlockAfterId = "global_profit_2",
            purchased = false
        });

        // ADD CUSTOMER
        items.Add(new UpgradeItemData
        {
            id = "add_customer_1",
            title = "Customer Lv1",
            desc = "Add +1 Customer",
            icon = customerIcon,
            cost = 10000,
            effectType = UpgradeEffectType.AddCustomer,
            addCustomerCount = 1,
            unlockAfterId = "",
            purchased = false
        });

        items.Add(new UpgradeItemData
        {
            id = "add_customer_2",
            title = "Customer Lv2",
            desc = "Add +2 Customer",
            icon = customerIcon,
            cost = 50000,
            effectType = UpgradeEffectType.AddCustomer,
            addCustomerCount = 2,
            unlockAfterId = "add_customer_1",
            purchased = false
        });

        return items;
    }
}