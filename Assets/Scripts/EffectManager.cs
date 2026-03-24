using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    [SerializeField] private SimpleEffectPool buildDonePool;
    [SerializeField] private SimpleEffectPool payPool;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayBuildDone(Vector3 position)
    {
        if (buildDonePool != null)
            buildDonePool.Play(position);
    }

    public void PlayPay(Vector3 position)
    {
        if (payPool != null)
            payPool.Play(position);
    }
}