using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/Action/GetIron", order = 1)]
public class A_GetIron : Action
{
    public override bool ProceduralPrecondition()
    {
        if (!IsRunning) //During Plannification
        {
            if (World.Instance.GetFurnacesWithIron().Count > 0) //Check if there are avaiable furnaces with iron
            {
                List<Furnace> furnaces = World.Instance.GetFurnacesWithIron();
                Target = furnaces[Random.Range(0, furnaces.Count)].gameObject;
                return true;
            }
        }
        else //During Execution (check if the Action is still valid)
        {
            //Target Destroyed
            if (Target == null)
                return false;

            //Iron taken by another AI
            if (!Target.GetComponent<Furnace>().CanPickUp())
                return false;
        }

        return true;
    }
    public override bool ExecuteAction()
    {
        if(IsAtTarget())
        {
            //Try to pick up the Iron from the furnace
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
