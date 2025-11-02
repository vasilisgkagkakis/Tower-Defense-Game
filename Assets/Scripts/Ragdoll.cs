using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] ragdollRigidbodies;
    Collider[] colList;
    Animator animator;
    NavMeshAgent navMeshAgent;

    //παιρνω οτι χρειαζομαι για αργοτερα
    private void Awake()
    {
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        colList = transform.GetComponentsInChildren<Collider>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        DisableRagdoll();
        //enable ragdoll after 3 seconds for testing purposes
        // StartCoroutine(EnableRagdollDelayed(transform.position, 3f));
    }

    //για καθε rigidbody που εχει, τα κανω kinematic ωστε να μην χρησιμοποιουν την βαρυτητα ακομα
    public void DisableRagdoll()
    {
        foreach (var rigidbody in ragdollRigidbodies)
        {
            rigidbody.isKinematic = true;
            animator.enabled = true;
        }
    }

    //κανω ολα τα rigidbody να χρησιμοποιουν την βαρυτητα ωστε να "πεσουν" σαν ragdoll
    public void EnableRagdoll(Vector3 hitpoint)
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
        
        Debug.Log($"🗑️ Cleaning up ragdoll: {gameObject.name}");
        
        // Remove from EntitySummoner tracking and destroy
        Enemy enemyScript = GetComponent<Enemy>();
        if (enemyScript != null)
        {
            EntitySummoner.RemoveEnemy(enemyScript);
        }
        else
        {
            // Fallback: just destroy
            Destroy(gameObject);
        }
    }
    //Η αναδρομική συνάρτηση που αλλάζει όλα τα layers του ζόμπι σε ragdoll ώστε να μην υπάρχουν πια colliders με τον παίχτη όταν πεθάνει
    void SetAllChildLayers(Transform parentTransform, string layerName)
    {
        parentTransform.gameObject.layer = LayerMask.NameToLayer(layerName);
        foreach (Transform childTransform in parentTransform)
        {
            SetAllChildLayers(childTransform, layerName);
        }
    }

    private IEnumerator EnableRagdollDelayed(Vector3 hitpoint, float delay)
    {
        yield return new WaitForSeconds(delay);
        EnableRagdoll(hitpoint);
        Destroy(gameObject, 3f); // Destroy the game object after 10 seconds to prevent memory leaks
    }
}
