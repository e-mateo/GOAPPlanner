public class Node
{
    int cost;
    Node parent;
    Action action;
    WorldState state;

    public int Cost {  get { return cost; } }
    public Action Action { get { return action; } }
    public Node Parent { get { return parent; } }
    public WorldState State { get { return state; } }


    public Node(Node parent, WorldState worldState, Action action)
    {
        this.parent = parent;
        this.state = worldState;
        this.action = action;

        if(parent != null && action != null)
        {
            this.cost = parent.cost + action.GetCost(parent.state);
        }
        else
        {
            this.cost = 0;
        }
    }
}
