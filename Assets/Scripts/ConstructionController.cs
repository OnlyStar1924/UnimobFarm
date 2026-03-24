using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ConstructionType constructionType;
    [SerializeField] private int level = 1;

    [Header("Config")]
    [SerializeField] private ConstructionConfigDatabaseSO configDatabase;

    [Header("UI")]
    [SerializeField] private Transform popupAnchor;

    [Header("Delivery")]
    [SerializeField] private Transform deliveryPoint;

    [Header("Product")]
    [SerializeField] private GameObject tomatoPrefab;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    private readonly List<GameObject> spawnedProducts = new List<GameObject>();
    private ConstructionUpgradeView upgradeView;
    private ConstructionUpgradeConfigSO config;
    private float extraProfitMultiplier = 1f;

    public ConstructionType ConstructionType => constructionType;
    public int Level => level;
    public int MaxLevel => config != null ? config.maxLevel : 10;
    public float GrowInterval => config != null ? config.growInterval : 2f;
    public int MaxProductCount => config != null ? config.maxProductCount : 3;

    private void Awake()
    {
        upgradeView = FindObjectOfType<ConstructionUpgradeView>();
    }

    public void Initialize(ConstructionType type)
    {
        constructionType = type;

        if (configDatabase == null)
            configDatabase = Resources.Load<ConstructionConfigDatabaseSO>("ConstructionConfigDatabase");

        if (configDatabase != null)
            config = configDatabase.GetConfig(constructionType);

        if (config == null)
            Debug.LogError($"Config not found for {constructionType}", this);

        StartCoroutine(GrowRoutine());
    }

    private void OnMouseDown()
    {
        if (upgradeView == null)
            upgradeView = FindObjectOfType<ConstructionUpgradeView>();

        if (upgradeView != null)
            upgradeView.Show(this);
    }

    private IEnumerator GrowRoutine()
    {
        while (true)
        {
            if (spawnedProducts.Count < MaxProductCount)
            {
                yield return new WaitForSeconds(GrowInterval);
                TrySpawnProduct();
            }
            else
            {
                yield return null;
            }
        }
    }

    private void TrySpawnProduct()
    {
        if (tomatoPrefab == null || spawnPoints.Count == 0) return;
        if (spawnedProducts.Count >= MaxProductCount) return;

        Transform point = spawnPoints[spawnedProducts.Count];
        GameObject product = Instantiate(tomatoPrefab, point.position, point.rotation, transform);
        spawnedProducts.Add(product);
    }

    public bool HasFullBatch()
    {
        return spawnedProducts.Count >= MaxProductCount;
    }

    public HarvestedItem HarvestBatch()
    {
        if (!HasFullBatch()) return null;

        int sellPriceSnapshot = GetBatchSellPrice();

        for (int i = 0; i < spawnedProducts.Count; i++)
        {
            if (spawnedProducts[i] != null)
                Destroy(spawnedProducts[i]);
        }

        spawnedProducts.Clear();

        return new HarvestedItem(constructionType, sellPriceSnapshot, MaxProductCount);
    }

    public Vector3 GetPopupWorldPosition()
    {
        if (popupAnchor != null)
            return popupAnchor.position;

        return transform.position + Vector3.up * 1.5f;
    }

    public Vector3 GetDeliveryPointPosition()
    {
        if (deliveryPoint != null)
            return deliveryPoint.position;

        return transform.position;
    }

    public string GetDisplayName()
    {
        if (config != null && !string.IsNullOrEmpty(config.displayName))
            return config.displayName;

        return constructionType.ToString();
    }

    public int GetPricePerTomato()
    {
        LevelData data = GetCurrentLevelData();
        return data != null ? data.pricePerTomato : 10;
    }

    public int GetBatchSellPrice()
    {
        return Mathf.RoundToInt(GetPricePerTomato() * MaxProductCount * extraProfitMultiplier);
    }

    public bool IsMaxLevel()
    {
        return level >= MaxLevel;
    }

    public int GetUpgradeCost()
    {
        if (IsMaxLevel()) return 0;

        LevelData data = GetCurrentLevelData();
        return data != null ? data.upgradeCost : 0;
    }

    public bool CanUpgrade()
    {
        if (IsMaxLevel()) return false;
        if (GameManager.Instance == null) return false;

        int cost = GetUpgradeCost();
        if (cost <= 0) return false;

        return GameManager.Instance.CurrentGold >= cost;
    }

    public bool TryUpgrade()
    {
        if (IsMaxLevel()) return false;
        if (GameManager.Instance == null) return false;

        int cost = GetUpgradeCost();
        if (cost <= 0) return false;

        bool success = GameManager.Instance.SpendGold(cost);
        if (!success) return false;

        level++;
        return true;
    }

    public void MultiplyProfit(float multiplier)
    {
        extraProfitMultiplier *= multiplier;
    }

    private LevelData GetCurrentLevelData()
    {
        if (config == null || config.levels == null) return null;

        for (int i = 0; i < config.levels.Count; i++)
        {
            if (config.levels[i].level == level)
                return config.levels[i];
        }

        return null;
    }
}