using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/Action/PutInChest", order = 1)]
public class A_PutInChest : Action
{
    public override bool ProceduralPrecondition()
    {
        if (!IsRunning)
        {
            if (World.Instance.Chests.Count > 0) //During Planning
            {
                Target = World.Instance.Chests[0].gameObject;
                return true;
            }
        }
        else //During Execution
        {
            //Target destroyed or invalid type
            if (Target == null || !Target.GetComponent<Chest>())
                return false;
        }

        return true;
    }

    public override bool ExecuteAction()
    {
        if(IsAtTarget())
        {
            Chest chest = Target.GetComponent<Chest>();
            if (chest)
            {
                chest.DepositIron();
                agent.GetComponent<InventaryUI>().ActivateIronUI(false);
                return true;
            }
        }

        return false;
    }
}
