using System.Collections;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private string isMoveParam = "IsMove";
    [SerializeField] private string isCarryMoveParam = "IsCarryMove";
    [SerializeField] private string isEmptyParam = "IsEmpty";

    [Header("Carry Visual")]
    [SerializeField] private DeliveryCarryVisual carryVisual;

    private Vector3 targetPosition;
    private bool isMoving;
    private bool isWaiting;
    private bool isCarrying;

    private CustomerSpawner spawner;
    private CustomerPool pool;

    public bool IsWaiting => isWaiting;

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
        if (animator == null)
            animator = GetComponentInChildren<Animator>(true);

        if (carryVisual == null)
            carryVisual = GetComponentInChildren<DeliveryCarryVisual>(true);

        if (carryVisual != null)
            carryVisual.HideAll();
    }

    private void Update()
    {
        UpdateMove();
        UpdateAnimation();
    }

    private void UpdateMove()
    {
        if (!isMoving) return;

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.0001f)
            transform.forward = direction.normalized;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) <= 0.05f)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

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

        isMoving = false;
        isWaiting = false;
        isCarrying = false;

        if (carryVisual != null)
            carryVisual.HideAll();
    }

    public void MoveToQueue(Vector3 queuePosition)
    {
        isWaiting = true;
        isCarrying = false;

        if (carryVisual != null)
            carryVisual.HideAll();

        targetPosition = queuePosition;
        isMoving = true;
    }

    public void CompletePurchase(Vector3 exitPosition)
    {
        isWaiting = false;
        isCarrying = true;

        if (carryVisual != null)
            carryVisual.ShowAmount(3);

        targetPosition = exitPosition;
        isMoving = true;

        StartCoroutine(ReturnAfterReach(exitPosition));
    }

    private IEnumerator ReturnAfterReach(Vector3 exitPosition)
    {
        while (Vector3.Distance(transform.position, exitPosition) > 0.05f)
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