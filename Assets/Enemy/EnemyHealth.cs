using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private int initialHealth = 60;

    private Bank bank;

    private int currentHealth;

    private void Start()
    {
        // Initialize the current health to the initial health.
        currentHealth = initialHealth;

        bank = GameObject.Find("Bank").GetComponent<Bank>();
    }

    private void UpdateHealth()
    {
        Debug.Log($"Current Health is: {currentHealth}");
        // Check if the health has reached zero or less.
        if (currentHealth <= 0)
        {
            onEnemyDeath();
        }
    }

    public void onEnemyDeath()
    {
        Debug.Log("Object is dead: " + currentHealth.ToString());

        // Disable the MeshRenderer and BoxCollider of this object
        MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }

        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        bank.AddCoins(20);
        Destroy(gameObject);
    }

    public bool IsAlive()
    {
        // Return whether the object is still alive (health > 0)
        return currentHealth > 0;
    }

    private void OnParticleCollision(GameObject other)
    {
        // Take 10 damage when a particle collides with the game object.
        TakeDamage(10);
    }

    private void TakeDamage(int damage)
    {
        // Reduce the current health by the specified damage amount.
        currentHealth -= damage;

        // Log a message indicating the current health.
        Debug.Log("Current health: " + currentHealth.ToString());

        // Update the health to reflect the new value.
        UpdateHealth();
    }
}
