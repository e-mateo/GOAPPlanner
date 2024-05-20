using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/Goal/FillChest", order = 1)]
public class G_FIllChest : Goal
{
    public override float GetPriority(Agent agent)
    {
        WorldState worldState = agent.GetWorldState();

        if (worldState.GetValue(StateType.HASIRON))
            return 1.0f;

        if (worldState.GetValue(StateType.IRONREADY) && (agent.CurrentTarget && agent.CurrentTarget.GetComponent<OreChunk>() && agent.NavMeshAgent.remainingDistance < 4f))
            return 0.5f;
        else if (worldState.GetValue(StateType.IRONREADY))
            return 0.75f;

        return 0f;
    }
}
