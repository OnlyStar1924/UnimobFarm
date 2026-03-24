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

    private Camera mainCamera;
    private RectTransform rectTransform;
    private BoxController currentBox;

    private Coroutine punchRoutine;
    private Vector3 buildButtonDefaultScale = Vector3.one;
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
            buildButtonDefaultScale = buildButtonRect.localScale;

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
            buildButtonRect.localScale = buildButtonDefaultScale;

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
            Debug.Log("Not enough gold!");
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
            buildButtonRect.localScale = buildButtonDefaultScale;
    }

    private void PlayBuildButtonPunch()
    {
        if (buildButtonRect == null) return;

        if (punchRoutine != null)
            StopCoroutine(punchRoutine);

        punchRoutine = StartCoroutine(PunchRoutine());
    }

    private IEnumerator PunchRoutine()
    {
        yield return ScaleTo(buildButtonDefaultScale * downScale, downDuration);
        yield return ScaleTo(buildButtonDefaultScale * upScale, upDuration);
        yield return ScaleTo(buildButtonDefaultScale, returnDuration);

        punchRoutine = null;
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