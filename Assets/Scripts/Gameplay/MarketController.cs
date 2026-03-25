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
    private readonly Dictionary<DeliveryController, int> reservedDockByDelivery = new();

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
        return customerQueue.Count > 0;
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

    public bool TryReserveDockForDelivery(DeliveryController delivery, out MarketDockPoint reservedDock)
    {
        reservedDock = null;

        if (delivery == null) return false;
        if (!HasWaitingCustomer()) return false;

        if (reservedDockByDelivery.TryGetValue(delivery, out int existingIndex))
        {
            if (existingIndex >= 0 && existingIndex < docks.Count)
            {
                reservedDock = docks[existingIndex];
                return true;
            }
        }

        for (int i = 0; i < customerQueue.Count; i++)
        {
            if (customerQueue[i] == null) continue;

            bool alreadyReserved = false;
            foreach (var pair in reservedDockByDelivery)
            {
                if (pair.Value == i)
                {
                    alreadyReserved = true;
                    break;
                }
            }

            if (alreadyReserved) continue;

            reservedDockByDelivery[delivery] = i;
            reservedDock = docks[i];
            return true;
        }

        return false;
    }

    public void ReleaseDeliveryReserve(DeliveryController delivery)
    {
        if (delivery == null) return;

        if (reservedDockByDelivery.ContainsKey(delivery))
            reservedDockByDelivery.Remove(delivery);
    }

    public bool ServeReservedCustomer(HarvestedItem item, DeliveryController delivery)
    {
        if (delivery == null) return false;
        if (item == null) return false;
        if (!reservedDockByDelivery.TryGetValue(delivery, out int queueIndex)) return false;
        if (queueIndex < 0 || queueIndex >= customerQueue.Count) return false;

        CustomerController customer = customerQueue[queueIndex];
        if (customer == null)
        {
            reservedDockByDelivery.Remove(delivery);
            RefreshQueuePositions();
            RebuildReservations();
            return false;
        }

        GameManager.Instance.AddGold(item.SellPrice);

        if (EffectManager.Instance != null)
        {
            if (queueIndex >= 0 && queueIndex < docks.Count)
            {
                MarketDockPoint dock = docks[queueIndex];
                if (dock != null && dock.CurrencyPoint != null)
                    EffectManager.Instance.PlayPay(dock.CurrencyPoint.position);
            }
        }

        customer.CompletePurchase(customerEnd.position);

        customerQueue.RemoveAt(queueIndex);
        reservedDockByDelivery.Remove(delivery);

        RefreshQueuePositions();
        RebuildReservations();

        return true;
    }

    public void RefreshQueuePositions()
    {
        for (int i = 0; i < customerQueue.Count; i++)
        {
            if (customerQueue[i] == null) continue;
            if (i >= docks.Count) continue;

            Transform point = docks[i].CustomerPoint;
            if (point != null)
                customerQueue[i].MoveToQueue(point.position);
        }
    }

    private void RebuildReservations()
    {
        List<DeliveryController> keys = new List<DeliveryController>(reservedDockByDelivery.Keys);

        for (int i = 0; i < keys.Count; i++)
        {
            int oldIndex = reservedDockByDelivery[keys[i]];
            int newIndex = Mathf.Clamp(oldIndex - 1, 0, Mathf.Max(0, customerQueue.Count - 1));

            if (customerQueue.Count == 0 || newIndex >= customerQueue.Count)
            {
                reservedDockByDelivery.Remove(keys[i]);
            }
            else
            {
                reservedDockByDelivery[keys[i]] = newIndex;
            }
        }
    }
}