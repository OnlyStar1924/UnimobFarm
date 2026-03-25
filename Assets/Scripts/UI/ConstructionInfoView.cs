using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConstructionInfoView : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text incomeText;
    [SerializeField] private Image iconImage;

    [Header("Icons")]
    [SerializeField] private Sprite wheatIcon;
    [SerializeField] private Sprite woodIcon;
    [SerializeField] private Sprite clayIcon;
    [SerializeField] private Sprite steelIcon;

    [Header("Offset")]
    [SerializeField] private Vector3 worldOffset;

    private Camera mainCamera;
    private ConstructionController targetConstruction;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
    }

    public void Bind(ConstructionController construction)
    {
        targetConstruction = construction;
        Show();
        Refresh();
        UpdatePosition();
    }

    private void LateUpdate()
    {
        if (targetConstruction == null) return;
        if (root != null && !root.activeSelf) return;

        Refresh();
        UpdatePosition();
    }

    public void Show()
    {
        if (root != null)
            root.SetActive(true);
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }

    private void Refresh()
    {
        if (targetConstruction == null) return;

        int batchSell = targetConstruction.GetBatchSellPrice();

        float incomePerMinute = 0f;
        if (targetConstruction.GrowInterval > 0f)
        {
            float batchesPerMinute = 60f / targetConstruction.GrowInterval;
            incomePerMinute = batchSell * batchesPerMinute;
        }

        if (goldText != null)
            goldText.text = NumberFormatter.Format(batchSell);

        if (incomeText != null)
            incomeText.text = NumberFormatter.Format(Mathf.RoundToInt(incomePerMinute)) + "/min";

        if (iconImage != null)
            iconImage.sprite = GetIcon(targetConstruction.ConstructionType);
    }

    private void UpdatePosition()
    {
        if (targetConstruction == null || rectTransform == null || mainCamera == null) return;

        Vector3 worldPos = targetConstruction.GetInfoWorldPosition() + worldOffset;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        if (screenPos.z <= 0f) return;

        rectTransform.position = screenPos;
    }

    private Sprite GetIcon(ConstructionType type)
    {
        switch (type)
        {
            case ConstructionType.Wheat: return wheatIcon;
            case ConstructionType.Wood: return woodIcon;
            case ConstructionType.Clay: return clayIcon;
            case ConstructionType.Steel: return steelIcon;
        }

        return null;
    }
}