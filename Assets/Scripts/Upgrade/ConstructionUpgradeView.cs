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

    [Header("Upgrade Button Punch")]
    [SerializeField] private RectTransform upgradeButtonRect;
    [SerializeField] private float downScale = 0.8f;
    [SerializeField] private float upScale = 1.1f;
    [SerializeField] private float downDuration = 0.06f;
    [SerializeField] private float upDuration = 0.08f;
    [SerializeField] private float returnDuration = 0.08f;

    [Header("Upgrade Button Shake")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeStrength = 12f;
    [SerializeField] private int shakeVibrato = 8;

    private Camera mainCamera;
    private RectTransform rectTransform;
    private ConstructionController currentConstruction;
    private bool canClose;

    private Coroutine animRoutine;
    private Vector3 upgradeButtonDefaultScale = Vector3.one;
    private Vector2 upgradeButtonDefaultAnchoredPos;
    private bool isProcessing;

    private float TotalPunchDuration => downDuration + upDuration + returnDuration;

    private void Awake()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();

        if (closeButton != null)
            closeButton.onClick.AddListener(OnClickClose);

        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnClickUpgrade);

        if (upgradeButtonRect == null && upgradeButton != null)
            upgradeButtonRect = upgradeButton.GetComponent<RectTransform>();

        if (upgradeButtonRect != null)
        {
            upgradeButtonDefaultScale = upgradeButtonRect.localScale;
            upgradeButtonDefaultAnchoredPos = upgradeButtonRect.anchoredPosition;
        }

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
        isProcessing = false;

        if (upgradeButton != null)
            upgradeButton.interactable = true;

        if (closeButton != null)
            closeButton.interactable = true;

        if (upgradeButtonRect != null)
        {
            upgradeButtonRect.localScale = upgradeButtonDefaultScale;
            upgradeButtonRect.anchoredPosition = upgradeButtonDefaultAnchoredPos;
        }

        if (root != null)
            root.SetActive(true);

        RefreshUI();
        UpdatePosition();

        StopAllCoroutines();
        StartCoroutine(EnableCloseAfterDelay());
    }

    public void Hide()
    {
        if (isProcessing) return;

        if (currentConstruction != null)
            currentConstruction.ShowInfoView();

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
        if (!canClose || isProcessing) return;

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
        if (isProcessing) return;
        if (currentConstruction == null) return;
        if (currentConstruction.IsMaxLevel()) return;

        if (!currentConstruction.CanUpgrade())
        {
            PlayUpgradeButtonShake();
            return;
        }

        StartCoroutine(CoUpgradeAfterPunch());
    }

    private IEnumerator CoUpgradeAfterPunch()
    {
        isProcessing = true;

        if (upgradeButton != null)
            upgradeButton.interactable = false;

        if (closeButton != null)
            closeButton.interactable = false;

        PlayUpgradeButtonPunch();

        yield return new WaitForSecondsRealtime(TotalPunchDuration);

        bool success = currentConstruction.TryUpgrade();

        if (!success)
            PlayUpgradeButtonShake();

        if (closeButton != null)
            closeButton.interactable = true;

        isProcessing = false;
        RefreshUI();
    }

    private void PlayUpgradeButtonPunch()
    {
        if (upgradeButtonRect == null) return;

        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(PunchRoutine());
    }

    private void PlayUpgradeButtonShake()
    {
        if (upgradeButtonRect == null) return;

        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator PunchRoutine()
    {
        yield return ScaleTo(upgradeButtonDefaultScale * downScale, downDuration);
        yield return ScaleTo(upgradeButtonDefaultScale * upScale, upDuration);
        yield return ScaleTo(upgradeButtonDefaultScale, returnDuration);

        animRoutine = null;
    }

    private IEnumerator ShakeRoutine()
    {
        float timer = 0f;
        float stepDuration = shakeDuration / Mathf.Max(1, shakeVibrato);

        while (timer < shakeDuration)
        {
            timer += stepDuration;

            float offsetX = Random.Range(-shakeStrength, shakeStrength);
            upgradeButtonRect.anchoredPosition = upgradeButtonDefaultAnchoredPos + new Vector2(offsetX, 0f);

            yield return new WaitForSecondsRealtime(stepDuration);
        }

        upgradeButtonRect.anchoredPosition = upgradeButtonDefaultAnchoredPos;
        animRoutine = null;
    }

    private IEnumerator ScaleTo(Vector3 targetScale, float duration)
    {
        if (upgradeButtonRect == null) yield break;

        Vector3 startScale = upgradeButtonRect.localScale;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);
            upgradeButtonRect.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        upgradeButtonRect.localScale = targetScale;
    }

    private void RefreshUI()
    {
        if (currentConstruction == null) return;

        if (levelText != null)
            levelText.text = $"Level {currentConstruction.Level}/{currentConstruction.MaxLevel}";

        if (productText != null)
            productText.text = currentConstruction.GetDisplayName();

        if (coinText != null)
            coinText.text = NumberFormatter.Format(currentConstruction.GetBatchSellPrice());

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
            upgradeButton.interactable = !isProcessing;
        }

        if (maxObject != null)
            maxObject.SetActive(isMax);

        if (!isMax && upgradeText != null)
            upgradeText.text = NumberFormatter.Format(currentConstruction.GetUpgradeCost());
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