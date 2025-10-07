using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public float MaxHealth;
    public float Health;
    public float Speed;
    public int ID;

    public float DistanceTravelled { get; private set; }
    private Vector3 lastPosition;

    public void Init()
    {
        Health = MaxHealth;
    }

    void Start()
    {
        lastPosition = transform.position;
        DistanceTravelled = 0f;
    }

    void Update()
    {
        DistanceTravelled += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        if(Health <= 0)
        {
            GetComponent<Ragdoll>().EnableRagdoll(transform.position);
            GetComponent<NavMeshAgent>().enabled = false;
        }
    }
}
