using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildProgressView : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text timeText;

    private Camera mainCamera;
    private RectTransform rectTransform;

    private void Awake()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    public void Show(Vector3 worldPosition)
    {
        gameObject.SetActive(true);
        UpdatePosition(worldPosition);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public IEnumerator PlayBuild(Vector3 worldPosition, float duration)
    {
        Show(worldPosition);

        float timer = duration;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            float remain = Mathf.Max(0f, timer);

            if (timeText != null)
                timeText.text = Mathf.CeilToInt(remain) + "s";

            if (fillImage != null)
                fillImage.fillAmount = remain / duration;

            UpdatePosition(worldPosition);
            yield return null;
        }

        if (timeText != null)
            timeText.text = "0s";

        if (fillImage != null)
            fillImage.fillAmount = 0f;

        Hide();
    }

    private void UpdatePosition(Vector3 worldPosition)
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null || rectTransform == null) return;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition);
        rectTransform.position = screenPos;
    }
}