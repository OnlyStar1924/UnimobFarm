using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConstructionBuildView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button buildButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image productIcon;

    [Header("Icons")]
    [SerializeField] private Sprite wheatIcon;
    [SerializeField] private Sprite woodIcon;
    [SerializeField] private Sprite clayIcon;
    [SerializeField] private Sprite steelIcon;

    private Camera mainCamera;
    private RectTransform rectTransform;
    private BoxController currentBox;

    private void Awake()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();

        if (buildButton != null)
            buildButton.onClick.AddListener(OnClickBuild);

        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);

        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (currentBox == null) return;
        UpdatePosition();
    }

    public void Show(BoxController box)
    {
        currentBox = box;
        gameObject.SetActive(true);

        RefreshUI();
        UpdatePosition();
    }

    public void Hide()
    {
        currentBox = null;
        gameObject.SetActive(false);
    }

    private void OnClickBuild()
    {
        if (currentBox == null) return;

        bool success = currentBox.TryUnlock();

        if (success)
            Hide();
        else
            Debug.Log("Not enough gold!");
    }

    private void RefreshUI()
    {
        if (currentBox == null) return;

        if (nameText != null)
            nameText.text = currentBox.GetDisplayName();

        if (costText != null)
            costText.text = currentBox.UnlockCost.ToString();

        if (productIcon != null)
            productIcon.sprite = GetIconByType(currentBox.ConstructionType);
    }

    private Sprite GetIconByType(ConstructionType type)
    {
        switch (type)
        {
            case ConstructionType.Wheat: return wheatIcon;
            case ConstructionType.Wood: return woodIcon;
            case ConstructionType.Clay: return clayIcon;
            case ConstructionType.Steel: return steelIcon;
            default: return null;
        }
    }

    private void UpdatePosition()
    {
        if (currentBox == null) return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null || rectTransform == null) return;

        Vector3 worldPos = currentBox.GetPopupWorldPosition();
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        if (screenPos.z <= 0f) return;

        rectTransform.position = screenPos;
    }
}