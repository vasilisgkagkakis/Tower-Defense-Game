using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private int currentCurrency = 200;
    [SerializeField] private TMP_Text currencyText;

    public static CurrencyManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateCurrencyUI();
    }

    public int GetCurrency()
    {
        return currentCurrency;
    }

    public bool SpendCurrency(int amount)
    {
        if (currentCurrency >= amount)
        {
            currentCurrency -= amount;
            UpdateCurrencyUI();
            RefreshUpgradeButton();
            return true;
        }
        return false;
    }

    public void AddCurrency(int amount)
    {
        currentCurrency += amount;
        UpdateCurrencyUI();
        RefreshUpgradeButton();
    }

    private void UpdateCurrencyUI()
    {
        if (currencyText != null)
        {
            currencyText.text = currentCurrency.ToString();
        }
    }

    private void RefreshUpgradeButton()
    {
        if (TowerDescriptionUI.Instance != null && TowerDescriptionUI.Instance.IsUIActive())
        {
            TowerDescriptionUI.Instance.RefreshUpgradeButton();
        }
    }
}