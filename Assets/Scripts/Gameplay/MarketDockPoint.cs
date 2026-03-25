using UnityEngine;

public class MarketDockPoint : MonoBehaviour
{
    [SerializeField] private Transform customerPoint;
    [SerializeField] private Transform deliveryPoint;
    [SerializeField] private Transform currencyPoint;

    public Transform CustomerPoint => customerPoint;
    public Transform DeliveryPoint => deliveryPoint;
    public Transform CurrencyPoint => currencyPoint;
}