using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeView : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private UpgradeItemView itemPrefab;

    [Header("Icons")]
    [SerializeField] private Sprite wheatIcon;
    [SerializeField] private Sprite woodIcon;
    [SerializeField] private Sprite clayIcon;
    [SerializeField] private Sprite steelIcon;
    [SerializeField] private Sprite customerIcon;
    [SerializeField] private Sprite globalProfitIcon;

    [Header("References")]
    [SerializeField] private CustomerSpawner customerSpawner;

    private readonly List<UpgradeItemData> items = new();
    private readonly List<UpgradeItemView> spawnedViews = new();

    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);

        if (root != null)
            root.SetActive(false);

        if (customerSpawner == null)
            customerSpawner = FindObjectOfType<CustomerSpawner>();

        CreateSampleData();
    }

    public void Show()
    {
        if (root != null)
            root.SetActive(true);

        SetAllConstructionInfoVisible(false);
        RefreshList();
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);

        SetAllConstructionInfoVisible(true);
    }

    private void SetAllConstructionInfoVisible(bool visible)
    {
        ConstructionController[] constructions = FindObjectsOfType<ConstructionController>();

        for (int i = 0; i < constructions.Length; i++)
        {
            if (visible)
                constructions[i].ShowInfoView();
            else
                constructions[i].HideInfoView();
        }
    }

    private void CreateSampleData()
    {
        items.Clear();

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
    }

    private void RefreshList()
    {
        ClearList();

        for (int i = 0; i < items.Count; i++)
        {
            UpgradeItemData item = items[i];

            if (!IsUnlocked(item))
                continue;

            UpgradeItemView view = Instantiate(itemPrefab, contentRoot);
            bool canBuy = !item.purchased && GameManager.Instance != null && GameManager.Instance.CurrentGold >= item.cost;
            view.Setup(item, OnClickBuyItem, canBuy);
            spawnedViews.Add(view);
        }
    }

    private void ClearList()
    {
        for (int i = 0; i < spawnedViews.Count; i++)
        {
            if (spawnedViews[i] != null)
                Destroy(spawnedViews[i].gameObject);
        }

        spawnedViews.Clear();
    }

    private bool IsUnlocked(UpgradeItemData item)
    {
        if (item.purchased) return false;
        if (string.IsNullOrEmpty(item.unlockAfterId)) return true;

        UpgradeItemData previous = items.Find(x => x.id == item.unlockAfterId);
        return previous != null && previous.purchased;
    }

    private void OnClickBuyItem(UpgradeItemData item)
    {
        if (item == null || item.purchased) return;
        if (GameManager.Instance == null) return;
        if (!GameManager.Instance.SpendGold(item.cost)) return;

        ApplyUpgrade(item);
        item.purchased = true;
        RefreshList();
    }

    private void ApplyUpgrade(UpgradeItemData item)
    {
        switch (item.effectType)
        {
            case UpgradeEffectType.ConstructionProfit:
            {
                ConstructionController[] constructions = FindObjectsOfType<ConstructionController>();
                for (int i = 0; i < constructions.Length; i++)
                {
                    if (constructions[i].ConstructionType == item.targetConstructionType)
                        constructions[i].MultiplyProfit(item.profitMultiplier);
                }
                break;
            }

            case UpgradeEffectType.GlobalProfit:
            {
                ConstructionController[] constructions = FindObjectsOfType<ConstructionController>();
                for (int i = 0; i < constructions.Length; i++)
                {
                    constructions[i].MultiplyProfit(item.profitMultiplier);
                }
                break;
            }

            case UpgradeEffectType.AddCustomer:
            {
                if (customerSpawner != null)
                    customerSpawner.AddCustomerCapacity(item.addCustomerCount);
                break;
            }
        }
    }
}