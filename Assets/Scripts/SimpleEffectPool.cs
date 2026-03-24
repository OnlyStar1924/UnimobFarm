using System.Collections.Generic;
using UnityEngine;

public class SimpleEffectPool : MonoBehaviour
{
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private int preloadCount = 5;
    [SerializeField] private float lifeTime = 1.5f;

    private readonly Queue<GameObject> pool = new();

    private void Awake()
    {
        for (int i = 0; i < preloadCount; i++)
        {
            GameObject obj = CreateNew();
            ReturnToPool(obj);
        }
    }

    private GameObject CreateNew()
    {
        GameObject obj = Instantiate(effectPrefab, transform);
        obj.SetActive(false);
        return obj;
    }

    public void Play(Vector3 position)
    {
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : CreateNew();

        obj.transform.SetParent(null);
        obj.transform.position = position;
        obj.transform.rotation = Quaternion.identity;
        obj.SetActive(true);

        StartCoroutine(ReturnAfterDelay(obj));
    }

    private System.Collections.IEnumerator ReturnAfterDelay(GameObject obj)
    {
        yield return new WaitForSeconds(lifeTime);
        ReturnToPool(obj);
    }

    private void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;

        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Enqueue(obj);
    }
}