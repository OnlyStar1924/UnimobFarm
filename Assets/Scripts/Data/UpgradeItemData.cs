using UnityEngine;

[System.Serializable]
public class UpgradeItemData
{
    public string id;
    public string title;
    public string desc;
    public Sprite icon;
    public int cost;
    public UpgradeEffectType effectType;
    public ConstructionType targetConstructionType;
    public float profitMultiplier = 2f;
    public int addCustomerCount = 0;
    public string unlockAfterId;
    public bool purchased;
}

public enum UpgradeEffectType
{
    ConstructionProfit,
    GlobalProfit,
    AddCustomer
}