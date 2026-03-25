using UnityEngine;
using UnityEngine.UI;

public class MainView : MonoBehaviour
{
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button addGoldButton;
    [SerializeField] private UpgradeView upgradeView;

    private void Awake()
    {
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnClickUpgrade);
        
        if (addGoldButton != null)
            addGoldButton.onClick.AddListener(OnClickAddGold);
    }

    private void OnClickUpgrade()
    {
        if (upgradeView != null)
            upgradeView.Show();
    }
    
    private void OnClickAddGold()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.AddGold(100000);
    }
}