using System.Collections;
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

    [Header("Button Punch")]
    [SerializeField] private RectTransform buildButtonRect;
    [SerializeField] private float downScale = 0.8f;
    [SerializeField] private float upScale = 1.1f;
    [SerializeField] private float downDuration = 0.06f;
    [SerializeField] private float upDuration = 0.08f;
    [SerializeField] private float returnDuration = 0.08f;

    [Header("Button Shake")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeStrength = 12f;
    [SerializeField] private int shakeVibrato = 8;

    private Camera mainCamera;
    private RectTransform rectTransform;
    private BoxController currentBox;

    private Coroutine animRoutine;
    private Vector3 buildButtonDefaultScale = Vector3.one;
    private Vector2 buildButtonDefaultAnchoredPos;
    private bool isProcessing;

    private float TotalPunchDuration => downDuration + upDuration + returnDuration;

    private void Awake()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();

        if (buildButton != null)
            buildButton.onClick.AddListener(OnClickBuild);

        if (closeButton != null)
            closeButton.onClick.AddListener(OnClickClose);

        if (buildButtonRect == null && buildButton != null)
            buildButtonRect = buildButton.GetComponent<RectTransform>();

        if (buildButtonRect != null)
        {
            buildButtonDefaultScale = buildButtonRect.localScale;
            buildButtonDefaultAnchoredPos = buildButtonRect.anchoredPosition;
        }

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
        isProcessing = false;

        if (buildButton != null)
            buildButton.interactable = true;

        if (closeButton != null)
            closeButton.interactable = true;

        if (buildButtonRect != null)
        {
            buildButtonRect.localScale = buildButtonDefaultScale;
            buildButtonRect.anchoredPosition = buildButtonDefaultAnchoredPos;
        }

        gameObject.SetActive(true);

        RefreshUI();
        UpdatePosition();
    }

    public void Hide()
    {
        if (isProcessing) return;

        currentBox = null;
        gameObject.SetActive(false);
    }

    private void OnClickClose()
    {
        if (isProcessing) return;
        Hide();
    }

    private void OnClickBuild()
    {
        if (isProcessing) return;
        if (currentBox == null) return;

        if (GameManager.Instance == null || GameManager.Instance.CurrentGold < currentBox.UnlockCost)
        {
            PlayBuildButtonShake();
            return;
        }

        StartCoroutine(CoBuildAfterPunch(currentBox));
    }

    private IEnumerator CoBuildAfterPunch(BoxController targetBox)
    {
        isProcessing = true;

        if (buildButton != null)
            buildButton.interactable = false;

        if (closeButton != null)
            closeButton.interactable = false;

        PlayBuildButtonPunch();

        yield return new WaitForSecondsRealtime(TotalPunchDuration);

        if (targetBox == null)
        {
            ResetViewState();
            yield break;
        }

        bool success = targetBox.TryUnlock();

        if (success)
        {
            currentBox = null;
            gameObject.SetActive(false);
            ResetViewState();
        }
        else
        {
            if (buildButton != null)
                buildButton.interactable = true;

            if (closeButton != null)
                closeButton.interactable = true;

            isProcessing = false;
            PlayBuildButtonShake();
        }
    }

    private void ResetViewState()
    {
        isProcessing = false;

        if (buildButton != null)
            buildButton.interactable = true;

        if (closeButton != null)
            closeButton.interactable = true;

        if (buildButtonRect != null)
        {
            buildButtonRect.localScale = buildButtonDefaultScale;
            buildButtonRect.anchoredPosition = buildButtonDefaultAnchoredPos;
        }
    }

    private void PlayBuildButtonPunch()
    {
        if (buildButtonRect == null) return;

        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(PunchRoutine());
    }

    private void PlayBuildButtonShake()
    {
        if (buildButtonRect == null) return;

        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator PunchRoutine()
    {
        yield return ScaleTo(buildButtonDefaultScale * downScale, downDuration);
        yield return ScaleTo(buildButtonDefaultScale * upScale, upDuration);
        yield return ScaleTo(buildButtonDefaultScale, returnDuration);

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
            buildButtonRect.anchoredPosition = buildButtonDefaultAnchoredPos + new Vector2(offsetX, 0f);

            yield return new WaitForSecondsRealtime(stepDuration);
        }

        buildButtonRect.anchoredPosition = buildButtonDefaultAnchoredPos;
        animRoutine = null;
    }

    private IEnumerator ScaleTo(Vector3 targetScale, float duration)
    {
        if (buildButtonRect == null) yield break;

        Vector3 startScale = buildButtonRect.localScale;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);
            buildButtonRect.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        buildButtonRect.localScale = targetScale;
    }

    private void RefreshUI()
    {
        if (currentBox == null) return;

        if (nameText != null)
            nameText.text = currentBox.GetDisplayName();

        if (costText != null)
            costText.text = NumberFormatter.Format(currentBox.UnlockCost);

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
        if (mainCamera == null || rectTransform == null || currentBox == null) return;

        Vector3 worldPos = currentBox.GetPopupWorldPosition();
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        if (screenPos.z <= 0f) return;

        rectTransform.position = screenPos;
    }
}