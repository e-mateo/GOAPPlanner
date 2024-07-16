using UnityEngine;
using UnityEngine.AI;

public struct Inventory
{
    public int currentOre;
    public int maxOre;
    public int oreReserved;
}


[RequireComponent(typeof(NavMeshAgent))]
public class Agent : MonoBehaviour
{
    protected NavMeshAgent navMeshAgent;
    public NavMeshAgent NavMeshAgent { get { return navMeshAgent; } }

    private GameObject currentTarget = null;
    public GameObject CurrentTarget { get { return currentTarget; } set { currentTarget = value; } }

    WorldState CurrentWorldState;

    public Inventory inventory;
    public WorldState GetWorldState() { return CurrentWorldState; }
    public void SetWorldState(WorldState newWorldState) { CurrentWorldState = newWorldState; }

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected void Start()
    {
        inventory.currentOre = 0;
        inventory.maxOre = 2;

        World.Instance.RegisterAgent(this);
        CurrentWorldState = Instantiate(GetComponent<GOAPPlanner>().startingWorldState);
    }
}
