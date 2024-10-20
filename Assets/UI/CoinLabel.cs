using System.Collections;
using System.Collections.Generic;
using TMPro; // Make sure you have TextMeshPro imported
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinLabel : MonoBehaviour
{
    private Bank bank; // Reference to the Bank script
    private TextMeshProUGUI coinLabel; // Reference to the TextMeshProUGUI component

    void Start()
    {
        // Find the GameObject "Bank"
        GameObject bankObject = GameObject.Find("Bank");

        // Get the "Bank" script component from the "Bank" GameObject
        if (bankObject != null)
        {
            bank = bankObject.GetComponent<Bank>();
        }
        else
        {
            Debug.LogError("Bank GameObject not found!");
        }

        // Find the GameObject with "Coin Label" (ensure this object has a TextMeshProUGUI component)
        GameObject coinLabelObject = GameObject.Find("Coin Label");

        // Get the TextMeshProUGUI component from the "Coin Label" GameObject
        if (coinLabelObject != null)
        {
            coinLabel = coinLabelObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("Coin Label GameObject not found!");
        }

        // Update the label with the current coins
        UpdateCoinLabel();
    }

    void Update()
    {
        // Continuously update the label with the current coin value
        UpdateCoinLabel();
    }

    void UpdateCoinLabel()
    {
        if (bank != null && coinLabel != null)
        {
            // Get the coins from the "Bank" script and show them in the label
            int coins = bank.GetCoins(); // Assuming Bank script has a GetCoins() method

            if (coins < 0)
            {
                coinLabel.text = $"Game Over";
                Debug.Break();
            }
            else
            {
                coinLabel.text = $"Coins: {coins}";
            }
        }
    }
}
