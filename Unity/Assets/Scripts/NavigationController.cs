using UnityEngine;
using UnityEngine.AI;

public class NavigationController : MonoBehaviour
{
    NavMeshAgent agent;
    Transform focusObject;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void GoTo(Vector3 target, Transform lookAt = null)
    {
        focusObject = lookAt;
        agent.isStopped = false;
        agent.SetDestination(target);
    }

    public void Stop()
    {
        agent.isStopped = true;
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            agent.isStopped = true;

            if (focusObject != null)
            {
                Vector3 dir = focusObject.position - transform.position;
                dir.y = 0;
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }
    }
}
