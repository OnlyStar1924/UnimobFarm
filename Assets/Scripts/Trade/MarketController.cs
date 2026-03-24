using System.Collections.Generic;
using UnityEngine;

public class MarketController : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Transform customerStart;
    [SerializeField] private Transform customerEnd;
    [SerializeField] private Transform deliveryEnd;
    [SerializeField] private List<MarketDockPoint> docks = new List<MarketDockPoint>();

    private readonly List<CustomerController> customerQueue = new();

    public Transform CustomerStart => customerStart;
    public Transform CustomerEnd => customerEnd;
    public Transform DeliveryEnd => deliveryEnd;

    public int MaxQueueCount => docks.Count;
    public int CurrentQueueCount => customerQueue.Count;

    public MarketDockPoint GetMainDock()
    {
        if (docks == null || docks.Count == 0) return null;
        return docks[0];
    }

    public bool HasFreeSlot()
    {
        return customerQueue.Count < docks.Count;
    }

    public bool HasWaitingCustomer()
    {
        return customerQueue.Count > 0 && customerQueue[0] != null && customerQueue[0].IsWaiting;
    }

    public bool EnqueueCustomer(CustomerController customer)
    {
        if (customer == null) return false;
        if (!HasFreeSlot()) return false;
        if (customerQueue.Contains(customer)) return false;

        customerQueue.Add(customer);
        RefreshQueuePositions();
        return true;
    }
    public void ServeNextCustomer(HarvestedItem item)
    {
        if (item == null) return;
        if (customerQueue.Count == 0) return;

        CustomerController customer = customerQueue[0];
        if (customer == null)
        {
            customerQueue.RemoveAt(0);
            RefreshQueuePositions();
            return;
        }

        GameManager.Instance.AddGold(item.SellPrice);

        if (docks.Count > 0 && docks[0] != null && docks[0].CurrencyPoint != null)
        {
            if (EffectManager.Instance != null)
                EffectManager.Instance.PlayPay(docks[0].CurrencyPoint.position);
        }

        customer.CompletePurchase(customerEnd.position);

        customerQueue.RemoveAt(0);
        RefreshQueuePositions();
    }

    public MarketDockPoint GetNextFreeDock()
    {
        if (!HasFreeSlot()) return null;
        return docks[customerQueue.Count];
    }

    public void RefreshQueuePositions()
    {
        for (int i = 0; i < customerQueue.Count; i++)
        {
            if (customerQueue[i] == null) continue;
            if (i >= docks.Count) continue;

            customerQueue[i].MoveToQueue(docks[i].CustomerPoint.position);
        }
    }
}