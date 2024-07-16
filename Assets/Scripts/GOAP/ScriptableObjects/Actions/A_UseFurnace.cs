using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/Action/UseFurnace", order = 1)]
public class A_UseFurnace : Action
{
    public override bool ProceduralPrecondition()
    {
        if (!IsRunning) //During Planning
        {
            int oreNeededToUseFurnace = 2;
            if (World.Instance.GetAvailableFurnaces(oreNeededToUseFurnace).Count > 0) //Check if there are avaiable furnance
            {
                return true;
            }
        }
        else //During Execution
        {
            //Target destroyed or invalid type
            if (agent.CurrentTarget == null || !agent.CurrentTarget.GetComponent<Furnace>())
                return false;

            if (!Target.GetComponent<Furnace>().CanCraft(agent.inventory.currentOre))
                return false;
        }

        return true;
    }
    public override void StartAction()
    {
        base.StartAction();

        //Find the nearest Furnace
        List<Furnace> Furnaces = World.Instance.GetAvailableFurnaces(2);
        if(Furnaces.Count > 0)
        {
            Furnace nearest = Furnaces[0];
            float minDistance = Vector3.Distance(agent.transform.position, nearest.transform.position);

            for (int i = 1; i < Furnaces.Count; i++)
            {
                float distance = Vector3.Distance(agent.transform.position, Furnaces[i].transform.position);
                if (distance < minDistance)
                {
                    nearest = Furnaces[i];
                    minDistance = distance;
                }
            }
            Target = nearest.gameObject;
            GoToTarget();
        }
    }
    public override bool ExecuteAction()
    {
        if(IsAtTarget())
        {
            Furnace furnace = Target.GetComponent<Furnace>();
            if(furnace && furnace.CanCraft(agent.inventory.currentOre))
            {
                agent.inventory.currentOre = furnace.TryCraft(agent.inventory.currentOre);
                agent.GetComponent<InventaryUI>().UpdateOreUI(agent.inventory.currentOre);
                return true;
            }
        }

        return false;
    }

}
