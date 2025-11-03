using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] ragdollRigidbodies;
    Collider[] colList;
    Animator animator;
    NavMeshAgent navMeshAgent;

    // Initial setup
    private void Awake()
    {
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        colList = transform.GetComponentsInChildren<Collider>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        DisableRagdoll();
    }

    // make rigidbodies kinematic and enable animator
    public void DisableRagdoll()
    {
        foreach (var rigidbody in ragdollRigidbodies)
        {
            rigidbody.isKinematic = true;
            animator.enabled = true;
        }
    }

    // make rigidbodies non-kinematic to enable ragdoll physics
    public void EnableRagdoll()
    {
        animator.enabled = false;
        SetAllChildLayers(transform, "RagdollEnemy");
        foreach (var rigidbody in ragdollRigidbodies)
        {
            rigidbody.isKinematic = false;
        }
        foreach (var item in colList)
        {
            item.isTrigger = false;
        }
        navMeshAgent.enabled = false;

        // Auto-cleanup ragdoll after 3 seconds
        StartCoroutine(DestroyRagdollAfterDelay(3f));
    }

    private IEnumerator DestroyRagdollAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Remove from EntitySummoner tracking and destroy
        Enemy enemyScript = GetComponent<Enemy>();
        if (enemyScript != null)
        {
            EntitySummoner.RemoveEnemy(enemyScript);
        }
        else
        {
            Debug.LogError($"Enemy script not found on ragdoll: {gameObject.name}");
            // Fallback: just destroy
            Destroy(gameObject);
        }
    }

    // Recursive function that changes all layers of the zombie to ragdoll so that there are no more colliders with the player when it dies
    void SetAllChildLayers(Transform parentTransform, string layerName)
    {
        parentTransform.gameObject.layer = LayerMask.NameToLayer(layerName);
        foreach (Transform childTransform in parentTransform)
        {
            SetAllChildLayers(childTransform, layerName);
        }
    }
}
