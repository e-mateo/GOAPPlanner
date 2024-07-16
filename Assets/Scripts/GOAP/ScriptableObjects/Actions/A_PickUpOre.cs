using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/Action/PickUpOre", order = 1)]
public class A_PickUpOre : Action
{
    public override bool ProceduralPrecondition()
    {
        if (!IsRunning) //During Planning
        {
            if (World.Instance.OreChunks.Count <= 0) //Check if there are still avaialble ore chucks
                return false;

            OreChunk chunk = World.Instance.OreChunks[Random.Range(0, World.Instance.OreChunks.Count)];
            if (chunk.HasOreAvailable()) //Check if the selected ore chuck still has ore
            {
                Target = chunk.gameObject;
                return true;
            }
        }
        else //During Execution
        {
            //Target destroyed or invalid type
            if (Target == null || !Target.GetComponent<OreChunk>())
                return false;
        }

        return true;
    }

    public override void StartAction()
    {
        base.StartAction();
        OreChunk ore = Target.GetComponent<OreChunk>();
        if (!ore) return;


        int oreToTake = agent.inventory.maxOre - agent.inventory.currentOre;
        if (ore.ReserveOre(oreToTake)) //Reserve the ore for this AI
        {
            agent.inventory.oreReserved = oreToTake;
        }
        else if (ore.ReserveOre(oreToTake - 1))
        {
            agent.inventory.oreReserved = oreToTake - 1;
        }
    }

    public override bool ExecuteAction()
    {
        if (IsAtTarget())
        {
            OreChunk oreChuck = Target.GetComponent<OreChunk>();
            if (!oreChuck)
                return false;

            if (oreChuck.PickUpOre(agent.inventory.oreReserved))
            {
                agent.inventory.currentOre += agent.inventory.oreReserved;
                agent.inventory.oreReserved = 0;
            }

            agent.GetComponent<InventaryUI>().UpdateOreUI(agent.inventory.currentOre);

            return true;
        }

        return false;
    }

    public override void EndAction()
    {
        base.EndAction();

        //In case we only have one Ore (we need two ore to make an iron)
        if (agent.inventory.currentOre < agent.inventory.maxOre)
            agent.GetWorldState().UpdateValue(StateType.HASORE, false);
    }

    public override void Abort()
    {
        //Unreserve the ore if the plan abort during this action
        if(Target)
        {
            OreChunk oreChuck = Target.GetComponent<OreChunk>();
            if (oreChuck != null && IsRunning)
            {
                oreChuck.UnReserveOre(agent.inventory.oreReserved);
                agent.inventory.oreReserved = 0;
            }
        }
        base.Abort();
    }
}
