using UnityEngine;
using UnityEngine.AI;

public class DeliveryController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float interactDistance = 0.35f;
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
    private MarketDockPoint reservedMarketDock;

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

    private bool IsMoving
    {
        get
        {
            if (agent == null) return false;
            if (agent.pathPending) return true;
            return agent.remainingDistance > agent.stoppingDistance;
        }
    }

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

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

        if (agent != null)
        {
            agent.stoppingDistance = interactDistance;
            agent.updateRotation = true;
            agent.updateUpAxis = true;
        }

        UpdateAnimation();
    }

    private void Update()
    {
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

    private void FindConstruction()
    {
        if (market == null || !market.HasWaitingCustomer())
            return;

        ConstructionController[] constructions = FindObjectsOfType<ConstructionController>();

        float bestDistance = float.MaxValue;
        ConstructionController bestTarget = null;

        for (int i = 0; i < constructions.Length; i++)
        {
            if (!constructions[i].HasFullBatch())
                continue;

            if (constructions[i].IsReserved)
                continue;

            float pathDistance = GetPathDistance(constructions[i].GetDeliveryPointPosition());
            if (pathDistance < bestDistance)
            {
                bestDistance = pathDistance;
                bestTarget = constructions[i];
            }
        }

        if (bestTarget != null)
        {
            bool reserved = bestTarget.TryReserve(this);
            if (!reserved) return;

            targetConstruction = bestTarget;
            MoveTo(targetConstruction.GetDeliveryPointPosition());
            currentState = State.MoveToConstruction;
        }
    }

    private void CheckArriveConstruction()
    {
        if (agent == null || IsMoving) return;

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

        if (targetConstruction == null)
        {
            currentState = State.Idle;
            return;
        }

        carryingItem = targetConstruction.HarvestBatch(this);

        if (carryingItem == null)
        {
            targetConstruction.ReleaseReserve(this);
            targetConstruction = null;
            currentState = State.Idle;
            return;
        }

        if (carryVisual != null)
            carryVisual.ShowAmount(carryingItem.Amount);

        if (market != null && market.TryReserveDockForDelivery(this, out reservedMarketDock))
        {
            if (reservedMarketDock != null && reservedMarketDock.DeliveryPoint != null)
            {
                MoveTo(reservedMarketDock.DeliveryPoint.position);
                currentState = State.MoveToMarket;
                return;
            }
        }

        currentState = State.Idle;
    }

    private void CheckArriveMarket()
    {
        if (agent == null || IsMoving) return;

        bool sold = false;

        if (market != null && carryingItem != null)
            sold = market.ServeReservedCustomer(carryingItem, this);

        if (sold)
        {
            carryingItem = null;

            if (carryVisual != null)
                carryVisual.HideAll();
        }

        reservedMarketDock = null;
        targetConstruction = null;

        if (market != null)
            market.ReleaseDeliveryReserve(this);

        if (market != null && market.DeliveryEnd != null)
        {
            MoveTo(market.DeliveryEnd.position);
            currentState = State.ReturnToEnd;
        }
        else
        {
            currentState = State.Idle;
        }
    }

    private void CheckArriveEnd()
    {
        if (agent == null || IsMoving) return;
        currentState = State.Idle;
    }

    private void MoveTo(Vector3 destination)
    {
        if (agent == null) return;

        agent.isStopped = false;
        agent.SetDestination(destination);
    }

    private float GetPathDistance(Vector3 destination)
    {
        if (agent == null) return float.MaxValue;

        NavMeshPath path = new NavMeshPath();
        if (!agent.CalculatePath(destination, path))
            return float.MaxValue;

        if (path.status != NavMeshPathStatus.PathComplete)
            return float.MaxValue;

        float total = 0f;
        for (int i = 1; i < path.corners.Length; i++)
        {
            total += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }

        return total;
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        bool isCarrying = carryingItem != null;
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
}