using System.Collections;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ConstructionType constructionType;
    [SerializeField] private int unlockCost = 1000;

    [Header("Animation")]
    [SerializeField] private Animation boxAnimation;
    [SerializeField] private string openAnimationName = "BoxOpen";

    [Header("Build Time")]
    [SerializeField] private float buildDuration = 3f;

    [Header("References")]
    [SerializeField] private ConstructionBuildView buildView;
    [SerializeField] private BuildProgressView buildProgressView;
    [SerializeField] private Transform popupAnchor;
    [SerializeField] private GameObject constructionPrefab;
    [SerializeField] private Transform constructionSpawnPoint;

    private bool isOpened;

    public ConstructionType ConstructionType => constructionType;
    public int UnlockCost => unlockCost;
    public bool IsOpened => isOpened;

    private void Awake()
    {
        if (boxAnimation == null)
            boxAnimation = GetComponent<Animation>();
    }

    private void OnMouseDown()
    {
        if (isOpened) return;

        if (buildView == null)
        {
            Debug.LogError($"{name}: buildView is NULL", this);
            return;
        }

        buildView.Show(this);
    }

    public bool TryUnlock()
    {
        if (isOpened) return false;
        if (GameManager.Instance == null) return false;

        bool success = GameManager.Instance.SpendGold(unlockCost);
        if (!success) return false;

        StartBuild();
        return true;
    }

    private void StartBuild()
    {
        if (isOpened) return;

        isOpened = true;
        StartCoroutine(BuildRoutine());
    }

    private IEnumerator BuildRoutine()
    {
        Vector3 buildViewWorldPos = GetPopupWorldPosition();

        if (boxAnimation != null)
            boxAnimation.Play(openAnimationName);

        if (buildProgressView != null)
        {
            yield return StartCoroutine(buildProgressView.PlayBuild(buildViewWorldPos, buildDuration));
        }
        else
        {
            yield return new WaitForSeconds(buildDuration);
        }

        SpawnConstruction();

        if (EffectManager.Instance != null)
            EffectManager.Instance.PlayBuildDone(transform.position);

        gameObject.SetActive(false);
    }

    private void SpawnConstruction()
    {
        if (constructionPrefab == null)
        {
            Debug.LogError($"{name}: constructionPrefab is NULL", this);
            return;
        }

        Vector3 spawnPosition = transform.position;
        Quaternion spawnRotation = Quaternion.identity;

        if (constructionSpawnPoint != null)
        {
            spawnPosition = constructionSpawnPoint.position;
            spawnRotation = constructionSpawnPoint.rotation;
        }

        GameObject constructionObj = Instantiate(constructionPrefab, spawnPosition, spawnRotation);

        ConstructionController construction = constructionObj.GetComponent<ConstructionController>();
        if (construction != null)
        {
            construction.Initialize(constructionType);
        }
    }

    public Vector3 GetPopupWorldPosition()
    {
        if (popupAnchor != null)
            return popupAnchor.position;

        return transform.position + Vector3.up * 1.5f;
    }

    public string GetDisplayName()
    {
        switch (constructionType)
        {
            case ConstructionType.Wheat: return "Wheat";
            case ConstructionType.Wood: return "Wood";
            case ConstructionType.Clay: return "Clay";
            case ConstructionType.Steel: return "Steel";
            default: return "Unknown";
        }
    }
}