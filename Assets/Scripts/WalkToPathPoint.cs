using UnityEngine;
using UnityEngine.AI;

public class WalkToPathPoint : MonoBehaviour
{
    public Transform targetPosition;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Ensure the point is on the NavMesh
        if (NavMesh.SamplePosition(targetPosition.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
