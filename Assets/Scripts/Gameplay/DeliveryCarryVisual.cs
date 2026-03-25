using System.Collections.Generic;
using UnityEngine;

public class DeliveryCarryVisual : MonoBehaviour
{
    [SerializeField] private List<GameObject> carryTomatoes = new List<GameObject>();

    private void Awake()
    {
        HideAll();
    }

    public void ShowAmount(int amount)
    {
        for (int i = 0; i < carryTomatoes.Count; i++)
        {
            if (carryTomatoes[i] != null)
                carryTomatoes[i].SetActive(i < amount);
        }
    }

    public void HideAll()
    {
        ShowAmount(0);
    }
}