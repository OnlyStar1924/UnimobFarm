using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ConstructionUpgradeView : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root;

    [Header("References")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text productText;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text upgradeText;
    [SerializeField] private GameObject maxObject;
    [SerializeField] private RectTransform frameRect;

    [Header("Position")]
    [SerializeField] private float screenYOffset = 80f;

    [Header("Close Delay")]
    [SerializeField] private float closeEnableDelay = 0.15f;

    private Camera mainCamera;
    private RectTransform rectTransform;
    private ConstructionController currentConstruction;
    private bool canClose;

    private void Awake()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();

        if (closeButton != null)
            closeButton.onClick.AddListener(OnClickClose);

        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnClickUpgrade);

        if (root != null)
            root.SetActive(false);
    }

    private void LateUpdate()
    {
        if (currentConstruction == null) return;

        UpdatePosition();
        RefreshUI();
    }

    public void Show(ConstructionController construction)
    {
        currentConstruction = construction;
        canClose = false;

        if (root != null)
            root.SetActive(true);

        RefreshUI();
        UpdatePosition();

        StopAllCoroutines();
        StartCoroutine(EnableCloseAfterDelay());
    }

    public void Hide()
    {
        currentConstruction = null;
        canClose = false;

        if (root != null)
            root.SetActive(false);
    }

    private IEnumerator EnableCloseAfterDelay()
    {
        yield return new WaitForSeconds(closeEnableDelay);
        canClose = true;
    }

    private void OnClickClose()
    {
        if (!canClose) return;

        if (IsPointerInsideFrame())
            return;

        Hide();
    }

    private bool IsPointerInsideFrame()
    {
        if (frameRect == null) return false;

        Vector2 screenPoint;

#if UNITY_EDITOR || UNITY_STANDALONE
        screenPoint = Input.mousePosition;
#else
        if (Input.touchCount == 0) return false;
        screenPoint = Input.GetTouch(0).position;
#endif

        return RectTransformUtility.RectangleContainsScreenPoint(frameRect, screenPoint, mainCamera);
    }

    private void OnClickUpgrade()
    {
        if (currentConstruction == null) return;

        bool success = currentConstruction.TryUpgrade();
        if (!success) return;

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (currentConstruction == null) return;

        if (levelText != null)
            levelText.text = $"Level {currentConstruction.Level}/{currentConstruction.MaxLevel}";

        if (productText != null)
            productText.text = currentConstruction.GetDisplayName();

        if (coinText != null)
            coinText.text = currentConstruction.GetBatchSellPrice().ToString();

        if (timeText != null)
            timeText.text = $"{currentConstruction.GrowInterval:0.#}s";

        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = currentConstruction.MaxLevel;
            progressSlider.value = currentConstruction.Level;
        }

        bool isMax = currentConstruction.IsMaxLevel();

        if (upgradeButton != null)
        {
            upgradeButton.gameObject.SetActive(!isMax);
            upgradeButton.interactable = currentConstruction.CanUpgrade();
        }

        if (maxObject != null)
            maxObject.SetActive(isMax);

        if (!isMax && upgradeText != null)
            upgradeText.text = currentConstruction.GetUpgradeCost().ToString();
    }

    private void UpdatePosition()
    {
        if (currentConstruction == null || mainCamera == null || rectTransform == null) return;

        Vector3 worldPos = currentConstruction.GetPopupWorldPosition();
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        if (screenPos.z <= 0f) return;

        screenPos.y += screenYOffset;
        rectTransform.position = screenPos;
    }
}