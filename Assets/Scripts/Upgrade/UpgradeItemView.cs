using System.Collections;
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

    [Header("Button Punch")]
    [SerializeField] private RectTransform buyButtonRect;
    [SerializeField] private float downScale = 0.8f;
    [SerializeField] private float upScale = 1.1f;
    [SerializeField] private float downDuration = 0.06f;
    [SerializeField] private float upDuration = 0.08f;
    [SerializeField] private float returnDuration = 0.08f;

    [Header("Button Shake")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeStrength = 12f;
    [SerializeField] private int shakeVibrato = 8;

    private UpgradeItemData data;
    private System.Action<UpgradeItemData> onBuy;

    private Coroutine animRoutine;
    private Vector3 defaultScale = Vector3.one;
    private Vector2 defaultAnchoredPos;
    private bool isProcessing;

    private float TotalPunchDuration => downDuration + upDuration + returnDuration;

    private void Awake()
    {
        if (buyButton != null)
            buyButton.onClick.AddListener(OnClickBuy);

        if (buyButtonRect == null && buyButton != null)
            buyButtonRect = buyButton.GetComponent<RectTransform>();

        if (buyButtonRect != null)
        {
            defaultScale = buyButtonRect.localScale;
            defaultAnchoredPos = buyButtonRect.anchoredPosition;
        }
    }

    public void Setup(UpgradeItemData itemData, System.Action<UpgradeItemData> onBuyCallback, bool canBuy)
    {
        data = itemData;
        onBuy = onBuyCallback;
        isProcessing = false;

        if (iconImage != null)
            iconImage.sprite = data.icon;

        if (titleText != null)
            titleText.text = data.title;

        if (descText != null)
            descText.text = data.desc;

        if (costText != null)
            costText.text = NumberFormatter.Format(data.cost);

        if (buyButtonRect != null)
        {
            buyButtonRect.localScale = defaultScale;
            buyButtonRect.anchoredPosition = defaultAnchoredPos;
        }

        if (buyButton != null)
            buyButton.interactable = true;
    }

    private void OnClickBuy()
    {
        if (isProcessing) return;
        if (data == null) return;

        if (GameManager.Instance == null || GameManager.Instance.CurrentGold < data.cost)
        {
            PlayShake();
            return;
        }

        StartCoroutine(CoBuyAfterPunch());
    }

    private IEnumerator CoBuyAfterPunch()
    {
        isProcessing = true;

        if (buyButton != null)
            buyButton.interactable = false;

        PlayPunch();

        yield return new WaitForSecondsRealtime(TotalPunchDuration);

        onBuy?.Invoke(data);
    }

    private void PlayPunch()
    {
        if (buyButtonRect == null) return;

        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(PunchRoutine());
    }

    private void PlayShake()
    {
        if (buyButtonRect == null) return;

        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator PunchRoutine()
    {
        yield return ScaleTo(defaultScale * downScale, downDuration);
        yield return ScaleTo(defaultScale * upScale, upDuration);
        yield return ScaleTo(defaultScale, returnDuration);

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
            buyButtonRect.anchoredPosition = defaultAnchoredPos + new Vector2(offsetX, 0f);

            yield return new WaitForSecondsRealtime(stepDuration);
        }

        buyButtonRect.anchoredPosition = defaultAnchoredPos;
        animRoutine = null;
    }

    private IEnumerator ScaleTo(Vector3 targetScale, float duration)
    {
        if (buyButtonRect == null) yield break;

        Vector3 startScale = buyButtonRect.localScale;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);
            buyButtonRect.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        buyButtonRect.localScale = targetScale;
    }
}