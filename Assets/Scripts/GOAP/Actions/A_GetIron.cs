using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/Action/GetIron", order = 1)]
public class A_GetIron : Action
{
    public override bool ProceduralPrecondition()
    {
        if (!IsRunning) //During Planning
        {
            if (World.Instance.GetFurnacesWithIron().Count > 0)
            {
                List<Furnace> furnaces = World.Instance.GetFurnacesWithIron();
                Target = furnaces[Random.Range(0, furnaces.Count)].gameObject;
                return true;
            }
        }
        else //During Execution
        {
            //Target Destroyed
            if (Target == null)
                return false;

            //Iron taken by someone else
            if (!Target.GetComponent<Furnace>().CanPickUp())
                return false;
        }

        return true;
    }
    public override bool ExecuteAction()
    {
        if(IsAtTarget())
        {
            Furnace furnace = Target.GetComponent<Furnace>();
            if (furnace && furnace.TryPickUp())
            {
                agent.GetComponent<InventaryUI>().ActivateIronUI(true);
                return true;
            }
        }

        return false;
    }

}
