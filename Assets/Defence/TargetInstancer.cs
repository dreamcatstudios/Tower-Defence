using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetInstancer : MonoBehaviour
{
    private Bank bank;

    private void Start()
    {
        bank = GameObject.Find("Bank").GetComponent<Bank>();
    }

    public bool CanInstanceTarget()
    {
        // Allow instantiation if the player has enough coins

        Debug.Log($"Current Coins: {bank.GetCoins()}");

        if (bank.GetCoins() > 0)
        {
            return true;
        }
        return false;
    }

    public void DeductCost()
    {
        // Deduct the cost of instantiation (modify based on your game logic)
        bank.SubtractCoins(20); // Example: Deduct 40 coins when a target is placed
    }
}
