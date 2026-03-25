using System.Collections;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private CustomerPool customerPool;
    [SerializeField] private MarketController market;
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private int maxCustomerCount = 1;

    private int activeCustomerCount;
    private bool isSpawning;

    private void Start()
    {
        EnsureCustomers();
    }

    public void AddCustomerCapacity(int amount)
    {
        maxCustomerCount += amount;
        EnsureCustomers();
    }

    public void NotifyCustomerLeft()
    {
        activeCustomerCount = Mathf.Max(0, activeCustomerCount - 1);
        EnsureCustomers();
    }

    public void EnsureCustomers()
    {
        if (isSpawning) return;
        if (market == null || customerPool == null) return;

        int targetCount = Mathf.Min(maxCustomerCount, market.MaxQueueCount);
        int missing = targetCount - activeCustomerCount;

        if (missing <= 0) return;

        StartCoroutine(SpawnCustomersRoutine(missing));
    }

    private IEnumerator SpawnCustomersRoutine(int count)
    {
        isSpawning = true;

        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(respawnDelay);

            if (market == null || customerPool == null) break;
            if (market.CustomerStart == null) break;
            if (!market.HasFreeSlot()) break;

            CustomerController customer = customerPool.Get();
            customer.transform.SetParent(null);
            customer.SetSpawner(this);
            customer.PrepareForSpawn(market.CustomerStart.position, market.CustomerStart.rotation);

            bool enqueued = market.EnqueueCustomer(customer);
            if (!enqueued)
            {
                customerPool.ReturnToPool(customer);
                break;
            }

            activeCustomerCount++;
        }

        isSpawning = false;
    }
}