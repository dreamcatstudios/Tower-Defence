using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private Bank bank;

    private void Start()
    {
        // Find the "Bank" game object and get the Bank script component
        bank = GameObject.Find("Bank").GetComponent<Bank>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter with: " + other.gameObject.name);

        // Check if the other collider has the "Enemy" tag
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Assuming bank is a reference to your bank or coin management system
            bank.SubtractCoins(50);
            Destroy(other.gameObject);
        }
    }
}
