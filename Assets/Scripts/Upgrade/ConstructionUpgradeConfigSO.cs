using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConstructionUpgradeConfig", menuName = "Game/Construction Upgrade Config")]
public class ConstructionUpgradeConfigSO : ScriptableObject
{
    public ConstructionType constructionType;
    public string displayName;
    public int maxLevel = 10;
    public float growInterval = 2f;
    public int maxProductCount = 3;
    public List<LevelData> levels = new List<LevelData>();
}

[System.Serializable]
public class LevelData
{
    public int level;
    public int upgradeCost;
    public int pricePerTomato;
}