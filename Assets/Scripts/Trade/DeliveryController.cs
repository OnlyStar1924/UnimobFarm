using UnityEngine;

public class DeliveryController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float interactDistance = 0.1f;
    [SerializeField] private float harvestTime = 0.5f;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private string isMoveParam = "IsMove";
    [SerializeField] private string isCarryMoveParam = "IsCarryMove";
    [SerializeField] private string isEmptyParam = "IsEmpty";

    [Header("Carry Visual")]
    [SerializeField] private DeliveryCarryVisual carryVisual;

    private MarketController market;
    private ConstructionController targetConstruction;
    private HarvestedItem carryingItem;

    private Vector3 targetPosition;
    private bool isMoving;

    private enum State
    {
        Idle,
        MoveToConstruction,
        Harvesting,
        MoveToMarket,
        ReturnToEnd
    }

    private State currentState;
    private float timer;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>(true);

        if (carryVisual == null)
            carryVisual = GetComponentInChildren<DeliveryCarryVisual>(true);
    }

    private void Start()
    {
        market = FindObjectOfType<MarketController>();
        currentState = State.Idle;

        if (carryVisual != null)
            carryVisual.HideAll();

        UpdateAnimation();
    }

    private void Update()
    {
        UpdateMove();

        switch (currentState)
        {
            case State.Idle:
                FindConstruction();
                break;

            case State.MoveToConstruction:
                CheckArriveConstruction();
                break;

            case State.Harvesting:
                UpdateHarvesting();
                break;

            case State.MoveToMarket:
                CheckArriveMarket();
                break;

            case State.ReturnToEnd:
                CheckArriveEnd();
                break;
        }

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

        if (Vector3.Distance(transform.position, targetPosition) <= interactDistance)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }

    private void FindConstruction()
    {
        ConstructionController[] constructions = FindObjectsOfType<ConstructionController>();

        foreach (var construction in constructions)
        {
            if (construction.HasFullBatch())
            {
                targetConstruction = construction;
                targetPosition = construction.transform.position;
                isMoving = true;
                currentState = State.MoveToConstruction;
                return;
            }
        }
    }

    private void CheckArriveConstruction()
    {
        if (isMoving) return;

        if (targetConstruction == null)
        {
            currentState = State.Idle;
            return;
        }

        timer = harvestTime;
        currentState = State.Harvesting;
    }

    private void UpdateHarvesting()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        carryingItem = targetConstruction.HarvestBatch();

        if (carryingItem != null && carryVisual != null)
        {
            carryVisual.ShowAmount(carryingItem.Amount);
        }

        MarketDockPoint dock = market != null ? market.GetMainDock() : null;
        if (carryingItem != null && dock != null && dock.DeliveryPoint != null)
        {
            targetPosition = dock.DeliveryPoint.position;
            isMoving = true;
            currentState = State.MoveToMarket;
        }
        else
        {
            currentState = State.Idle;
        }
    }

    private void CheckArriveMarket()
    {
        if (isMoving) return;

        if (market != null && carryingItem != null && market.HasWaitingCustomer())
        {
            market.ServeNextCustomer(carryingItem);
            carryingItem = null;

            if (carryVisual != null)
                carryVisual.HideAll();
        }

        if (market != null && market.DeliveryEnd != null)
        {
            targetPosition = market.DeliveryEnd.position;
            isMoving = true;
            currentState = State.ReturnToEnd;
        }
        else
        {
            currentState = State.Idle;
        }
    }

    private void CheckArriveEnd()
    {
        if (isMoving) return;
        currentState = State.Idle;
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        bool isCarrying = carryingItem != null;

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
}