using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float Health;
    public int ID;

    public float DistanceTravelled { get; private set; }
    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
        DistanceTravelled = 0f;
    }

    void Update()
    {
        DistanceTravelled += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        if (Health <= 0)
        {
            GetComponent<Ragdoll>().EnableRagdoll(transform.position);
            GetComponent<NavMeshAgent>().enabled = false;
        }
    }

    public void TakeDamage(float damage)
    {
        if (Health <= 0) return; // Already dead

        Debug.Log($"Enemy  Health: {Health}");
        Health -= damage;
        Debug.Log($"Enemy {name} took {damage} damage. Health: {Health}");

        if (Health <= 0)
        {
            Health = 0;
            Die();
        }
    }

    private void Die()
    {
        // Grant currency reward to player
        if (CurrencyManager.Instance != null)
        {
            // Give a fixed reward per enemy kill - you can adjust this value as needed
            int reward = 5;
            CurrencyManager.Instance.AddCurrency(reward);
        }
    }
}