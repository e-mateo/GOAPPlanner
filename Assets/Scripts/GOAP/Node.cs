public class Node
{
    int cost; //cost of the node (takes into account the cost of every parent's node)
    Node parent; //ref to the parent node
    Action action; //ref to the action associated to this node
    WorldState state;  //ref of the worldstate during this action used to simulate an agent world state during plannification

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
            this.cost = parent.cost + action.GetActionCost(parent.state);
        }
        else
        {
            this.cost = 0;
        }
    }
}
