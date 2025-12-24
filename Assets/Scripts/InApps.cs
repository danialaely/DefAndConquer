using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InApps : MonoBehaviour
{
    [SerializeField] private TMP_Text gemsText;
    [SerializeField] private TMP_Text coinsText;

    public GameObject GemInnerPanel;
    public GameObject CoinInnerPanel;

    [Header("Deck Toggles (1=Starter, 2=Bronze, 3=Silver, 4=Gold)")]
    public Toggle[] deckToggles;

    private int[] coinCost = { 0, 0, 0, 0 }; // only 2nd deck uses coins
    private int[] gemCost = { 0, 0, 1200, 1500 };   // only 3rd deck uses gems

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Gems"))
            PlayerPrefs.SetInt("Gems", 0);
        if (!PlayerPrefs.HasKey("Coins"))
            PlayerPrefs.SetInt("Coins", 0);

        UpdateUI();

        InitializeDecks();
    }

    public void GemBundleOne() => AddGems(200, 1.99f);
    public void GemBundleTwo() => AddGems(550, 2.99f);
    public void GemBundleThree() => AddGems(1200, 4.99f);
    public void GemBundleFour() => AddGems(2600, 6.99f);
    public void GemBundleFive() => AddGems(7000, 11.99f);
    public void GemBundleSix() => AddGems(16000, 19.99f);

    public void CoinBundleOne() => AddCoins(200, 1.99f);
    public void CoinBundleTwo() => AddCoins(200, 2.99f);
    public void CoinBundleThree() => AddCoins(200, 3.99f);
    public void CoinBundleFour() => AddCoins(200, 6.99f);
    public void CoinBundleFive() => AddCoins(200, 11.99f);
    public void CoinBundleSix() => AddCoins(200, 19.99f);

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

    void InitializeDecks()
    {
        int coins = PlayerPrefs.GetInt("Coins", 0);
        int gems = PlayerPrefs.GetInt("Gems", 0);

        for (int i = 0; i < deckToggles.Length; i++)
        {
            bool unlocked = PlayerPrefs.GetInt($"Deck{i}_Unlocked", i == 0 ? 1 : 0) == 1;

            bool canAfford =
                (!unlocked && coinCost[i] > 0 && coins >= coinCost[i]) ||
                (!unlocked && gemCost[i] > 0 && gems >= gemCost[i]);

            deckToggles[i].interactable = unlocked || canAfford;
            deckToggles[i].isOn = PlayerPrefs.GetInt("SelectedDeck", 0) == i;
        }
    }

    public void OnDeckToggleChanged(int index)
    {
        // Only respond if toggle was turned ON
        if (!deckToggles[index].isOn) return;

        bool unlocked = PlayerPrefs.GetInt($"Deck{index}_Unlocked", index == 0 ? 1 : 0) == 1;

        if (!unlocked)
            TryUnlockDeck(index);
        else
            SelectDeck(index);
    }

    void TryUnlockDeck(int index)
    {
        int coins = PlayerPrefs.GetInt("Coins", 0);
        int gems = PlayerPrefs.GetInt("Gems", 0);

        if (coinCost[index] > 0 && coins >= coinCost[index])
        {
            PlayerPrefs.SetInt("Coins", coins - coinCost[index]);
            UnlockDeck(index);
        }
        else if (gemCost[index] > 0 && gems >= gemCost[index])
        {
            PlayerPrefs.SetInt("Gems", gems - gemCost[index]);
            UnlockDeck(index);
        }
        else
        {
            Debug.Log("❌ Not enough currency to unlock this deck!");
            deckToggles[index].isOn = false;
        }
    }

    void UnlockDeck(int index)
    {
        PlayerPrefs.SetInt($"Deck{index}_Unlocked", 1);
        deckToggles[index].interactable = true;
        SelectDeck(index);
        Debug.Log($"✅ Unlocked and selected Deck {index + 1}");
    }

    void SelectDeck(int index)
    {
        PlayerPrefs.SetInt("SelectedDeck", index);
        Debug.Log($"🎴 Selected Deck {index + 1}");
    }

    public void ClickGemInShop() 
    {
        GemInnerPanel.SetActive(true);
        CoinInnerPanel.SetActive(false);
    }

    public void ClickCoinInShop()
    {
        CoinInnerPanel.SetActive(true);
        GemInnerPanel.SetActive(false);
    }

}
