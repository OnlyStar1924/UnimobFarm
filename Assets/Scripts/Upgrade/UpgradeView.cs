using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeView : MonoBehaviour
{
    public static bool IsAnyUpgradeViewOpen;

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

        IsAnyUpgradeViewOpen = true;
        SetAllConstructionInfoVisible(false);
        RefreshList();
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);

        IsAnyUpgradeViewOpen = false;
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
        items.AddRange(UpgradeSampleData.Create(
            wheatIcon,
            woodIcon,
            clayIcon,
            steelIcon,
            customerIcon,
            globalProfitIcon
        ));
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