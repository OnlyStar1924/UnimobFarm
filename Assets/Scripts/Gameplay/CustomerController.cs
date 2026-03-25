using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CustomerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float interactDistance = 0.2f;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private string isMoveParam = "IsMove";
    [SerializeField] private string isCarryMoveParam = "IsCarryMove";
    [SerializeField] private string isEmptyParam = "IsEmpty";

    [Header("Carry Visual")]
    [SerializeField] private DeliveryCarryVisual carryVisual;

    private bool isWaiting;
    private bool isCarrying;

    private CustomerSpawner spawner;
    private CustomerPool pool;

    public bool IsWaiting => isWaiting;

    private bool IsMoving
    {
        get
        {
            if (agent == null) return false;
            if (agent.pathPending) return true;
            return agent.remainingDistance > agent.stoppingDistance;
        }
    }

    public void SetSpawner(CustomerSpawner customerSpawner)
    {
        spawner = customerSpawner;
    }

    public void SetPool(CustomerPool customerPool)
    {
        pool = customerPool;
    }

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>(true);

        if (carryVisual == null)
            carryVisual = GetComponentInChildren<DeliveryCarryVisual>(true);

        if (carryVisual != null)
            carryVisual.HideAll();

        if (agent != null)
            agent.stoppingDistance = interactDistance;
    }

    private void Update()
    {
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        bool isMoving = IsMoving;

        animator.SetBool(isMoveParam, false);
        animator.SetBool(isCarryMoveParam, false);

        if (isCarrying)
        {
            animator.SetBool(isEmptyParam, false);

            if (isMoving)
                animator.SetBool(isCarryMoveParam, true);
        }
        else
        {
            animator.SetBool(isEmptyParam, true);

            if (isMoving)
                animator.SetBool(isMoveParam, true);
        }
    }

    public void PrepareForSpawn(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        isWaiting = false;
        isCarrying = false;

        if (carryVisual != null)
            carryVisual.HideAll();

        if (agent != null)
        {
            agent.Warp(spawnPosition);
            agent.ResetPath();
        }
    }

    public void MoveToQueue(Vector3 queuePosition)
    {
        isWaiting = true;
        isCarrying = false;

        if (carryVisual != null)
            carryVisual.HideAll();

        if (agent != null)
        {
            agent.isStopped = false;
            agent.SetDestination(queuePosition);
        }
    }

    public void CompletePurchase(Vector3 exitPosition)
    {
        isWaiting = false;
        isCarrying = true;

        if (carryVisual != null)
            carryVisual.ShowAmount(3);

        if (agent != null)
        {
            agent.isStopped = false;
            agent.SetDestination(exitPosition);
        }

        StartCoroutine(ReturnAfterReach(exitPosition));
    }

    private IEnumerator ReturnAfterReach(Vector3 exitPosition)
    {
        while (agent != null && (agent.pathPending || agent.remainingDistance > agent.stoppingDistance))
        {
            yield return null;
        }

        spawner?.NotifyCustomerLeft();

        if (pool != null)
            pool.ReturnToPool(this);
        else
            gameObject.SetActive(false);
    }
}