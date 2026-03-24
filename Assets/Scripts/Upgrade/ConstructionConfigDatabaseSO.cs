using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConstructionConfigDatabase", menuName = "Game/Construction Config Database")]
public class ConstructionConfigDatabaseSO : ScriptableObject
{
    public List<ConstructionUpgradeConfigSO> configs = new List<ConstructionUpgradeConfigSO>();

    public ConstructionUpgradeConfigSO GetConfig(ConstructionType type)
    {
        for (int i = 0; i < configs.Count; i++)
        {
            if (configs[i] != null && configs[i].constructionType == type)
                return configs[i];
        }

        return null;
    }
}