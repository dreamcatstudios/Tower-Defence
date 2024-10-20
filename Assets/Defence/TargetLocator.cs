using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLocator : MonoBehaviour
{
    GameObject target;
    private ParticleSystem particleSystem;

    // Add a maxRange variable to limit shooting range
    [SerializeField]
    public float maxRange = 30f; // Set a desired range, e.g., 30 units

    void Start()
    {
        // Get the ParticleSystem component attached to this object
        particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        FindClosestTarget();

        // Check if the target exists and is within range
        if (
            target != null
            && Vector3.Distance(transform.position, target.transform.position) <= maxRange
        )
        {
            // Rotate to look towards the closest target
            transform.rotation = Quaternion.LookRotation(
                target.transform.position - transform.position
            );

            // Ensure the particle system is enabled and firing
            if (!particleSystem.isPlaying)
            {
                particleSystem.Play();
            }
        }
        else
        {
            // If no valid target, stop the particle system
            if (particleSystem.isPlaying)
            {
                particleSystem.Stop();
            }
        }
    }

    void FindClosestTarget()
    {
        // Get all objects with the "Enemy" tag
        GameObject[] players = GameObject.FindGameObjectsWithTag("Enemy");

        if (players.Length == 0)
        {
            target = null;
            return;
        }

        // Assume no closest player initially
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        // Iterate through the players to find the closest one with health > 0 and within range
        foreach (GameObject player in players)
        {
            // Look for Health in the player or its children
            EnemyHealth playerHealth = player.GetComponentInChildren<EnemyHealth>();

            // If the player exists and has health > 0
            if (playerHealth != null && playerHealth.IsAlive())
            {
                float distanceToPlayer = Vector3.Distance(
                    transform.position,
                    player.transform.position
                );

                // Check if the player is within the maximum range
                if (distanceToPlayer < closestDistance && distanceToPlayer <= maxRange)
                {
                    closestDistance = distanceToPlayer;
                    closestPlayer = player;
                }
            }
        }

        // Set the closest player as the target if any
        target = closestPlayer;
    }
}
