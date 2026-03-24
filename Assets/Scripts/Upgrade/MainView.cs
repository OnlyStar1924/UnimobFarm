using UnityEngine;
using UnityEngine.UI;

public class MainView : MonoBehaviour
{
    [SerializeField] private Button upgradeButton;
    [SerializeField] private UpgradeView upgradeView;

    private void Awake()
    {
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnClickUpgrade);
    }

    private void OnClickUpgrade()
    {
        if (upgradeView != null)
            upgradeView.Show();
    }
}