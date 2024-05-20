using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/Goal/ForgeIron", order = 1)]
public class G_ForgeIron : Goal
{
    public override float GetPriority(Agent agent)
    {
        WorldState worldState = agent.GetWorldState();

        if (!worldState.GetValue(StateType.IRONREADY))
            return 1.0f;

        if(agent.inventory.currentOre > 0 || agent.CurrentTarget.GetComponent<OreChunk>())
            return 0.6f;

        return 0f;
    }
}
