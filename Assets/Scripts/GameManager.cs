using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private int currentGold = 10000;
    [SerializeField] private TMP_Text goldText;

    public int CurrentGold => currentGold;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateGoldUI();
    }

    public bool SpendGold(int amount)
    {
        if (currentGold < amount) return false;

        currentGold -= amount;
        UpdateGoldUI();
        return true;
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateGoldUI();
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = currentGold.ToString();
    }
}