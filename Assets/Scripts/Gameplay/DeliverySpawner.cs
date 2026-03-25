using System.Collections.Generic;
using UnityEngine;

public class DeliverySpawner : MonoBehaviour
{
    [SerializeField] private DeliveryController deliveryPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<DeliveryController> spawnedDeliveries = new List<DeliveryController>();

    public void SpawnDelivery()
    {
        if (deliveryPrefab == null || spawnPoint == null) return;

        DeliveryController delivery = Instantiate(deliveryPrefab, spawnPoint.position, spawnPoint.rotation);
        spawnedDeliveries.Add(delivery);
    }
}