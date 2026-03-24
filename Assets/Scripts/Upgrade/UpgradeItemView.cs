using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemView : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private Button buyButton;
    [SerializeField] private TMP_Text costText;

    private UpgradeItemData data;
    private System.Action<UpgradeItemData> onBuy;

    private void Awake()
    {
        if (buyButton != null)
            buyButton.onClick.AddListener(OnClickBuy);
    }

    public void Setup(UpgradeItemData itemData, System.Action<UpgradeItemData> onBuyCallback, bool canBuy)
    {
        data = itemData;
        onBuy = onBuyCallback;

        if (iconImage != null)
            iconImage.sprite = data.icon;

        if (titleText != null)
            titleText.text = data.title;

        if (descText != null)
            descText.text = data.desc;

        if (costText != null)
            costText.text = NumberFormatter.Format(data.cost);

        if (buyButton != null)
            buyButton.interactable = canBuy;
    }

    private void OnClickBuy()
    {
        if (data == null) return;
        onBuy?.Invoke(data);
    }
}