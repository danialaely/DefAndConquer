using UnityEngine;
using TMPro;

public class InApps : MonoBehaviour
{
    [SerializeField] private TMP_Text gemsText;
    [SerializeField] private TMP_Text coinsText;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Gems"))
            PlayerPrefs.SetInt("Gems", 0);
        if (!PlayerPrefs.HasKey("Coins"))
            PlayerPrefs.SetInt("Coins", 0);

        UpdateUI();
    }

    public void GemBundleOne() => AddGems(100, 0.99f);
    public void GemBundleTwo() => AddGems(250, 1.99f);
    public void GemBundleThree() => AddGems(700, 4.99f);
    public void GemBundleFour() => AddGems(1500, 9.99f);
    public void GemBundleFive() => AddGems(3500, 19.99f);
    public void GemBundleSix() => AddGems(10000, 49.99f);

    public void CoinBundleOne() => AddCoins(1000, 0.99f);
    public void CoinBundleTwo() => AddCoins(2500, 1.99f);
    public void CoinBundleThree() => AddCoins(7000, 4.99f);
    public void CoinBundleFour() => AddCoins(15000, 9.99f);
    public void CoinBundleFive() => AddCoins(35000, 19.99f);
    public void CoinBundleSix() => AddCoins(100000, 49.99f);

    private void AddGems(int amount, float price)
    {
        int current = PlayerPrefs.GetInt("Gems");
        current += amount;
        PlayerPrefs.SetInt("Gems", current);
        PlayerPrefs.Save();

        Debug.Log($"💎 Purchased {amount} Gems for ${price}");
        UpdateUI();
    }

    private void AddCoins(int amount, float price)
    {
        int current = PlayerPrefs.GetInt("Coins");
        current += amount;
        PlayerPrefs.SetInt("Coins", current);
        PlayerPrefs.Save();

        Debug.Log($"🪙 Purchased {amount} Coins for ${price}");
        UpdateUI();
    }

    private void UpdateUI()
    {
        gemsText.text = $"Gems: {PlayerPrefs.GetInt("Gems")}";
        coinsText.text = $"Coins: {PlayerPrefs.GetInt("Coins")}";
    }
}
