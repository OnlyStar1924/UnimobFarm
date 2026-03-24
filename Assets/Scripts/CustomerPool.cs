using System.Collections.Generic;
using UnityEngine;

public class CustomerPool : MonoBehaviour
{
    [SerializeField] private CustomerController customerPrefab;
    [SerializeField] private int preloadCount = 5;

    private readonly Queue<CustomerController> pool = new();

    private void Awake()
    {
        Preload();
    }

    private void Preload()
    {
        for (int i = 0; i < preloadCount; i++)
        {
            CustomerController customer = CreateNew();
            ReturnToPool(customer);
        }
    }

    private CustomerController CreateNew()
    {
        CustomerController customer = Instantiate(customerPrefab, transform);
        customer.gameObject.SetActive(false);
        customer.SetPool(this);
        return customer;
    }

    public CustomerController Get()
    {
        if (pool.Count == 0)
        {
            CustomerController newCustomer = CreateNew();
            return newCustomer;
        }

        CustomerController customer = pool.Dequeue();
        customer.gameObject.SetActive(true);
        return customer;
    }

    public void ReturnToPool(CustomerController customer)
    {
        if (customer == null) return;

        customer.gameObject.SetActive(false);
        customer.transform.SetParent(transform);
        pool.Enqueue(customer);
    }
}