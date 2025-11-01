using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private int currentCurrency = 400;
    [SerializeField] private TMP_Text currencyText;

    public static CurrencyManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
            return true;
        }
        return false;
    }

    public void AddCurrency(int amount)
    {
        currentCurrency += amount;
        UpdateCurrencyUI();
    }

    private void UpdateCurrencyUI()
    {
        if (currencyText != null)
        {
            currencyText.text = currentCurrency.ToString();
        }
    }
}