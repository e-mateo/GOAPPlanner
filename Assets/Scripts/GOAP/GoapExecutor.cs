using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoapExecutor : MonoBehaviour
{
    GOAPPlanner goapPlanner;
    Queue<Node> currentPlan;
    Goal currentGoal;
    Action currentAction;
    Agent agent;

    [SerializeField] TMP_Text goalText;
    [SerializeField] TMP_Text actionText;

    private void Start()
    {
        goapPlanner = GetComponent<GOAPPlanner>();
        agent = GetComponent<Agent>();
    }

    private void Update()
    {
        CheckForNewGoal();
        if (!currentAction)
            return;

        if(!currentAction.IsRunning)
        {
            currentAction.StartAction();
        }
        else
        {
            if (!currentAction.IsValid(agent.GetWorldState()))
            {
                currentAction.Abort();
                ComputeNewPlan();
                return;
            }

            bool completed = currentAction.ExecuteAction();
            if (completed)
            {
                CompleteAction();
            }
        }
    }

    private void CompleteAction()
    {
        agent.SetWorldState(currentAction.ApplyEffect(agent.GetWorldState()));
        currentAction.EndAction();

        currentAction = GetNextAction();
        if (currentAction == null) //Has Reached the end of the goal
        {
            currentGoal.PostValidateGoal(agent.GetWorldState());
            CheckForNewGoal();
        }
    }

    private void ComputeNewPlan()
    {
        Node startingNode = new Node(null, agent.GetWorldState(), null);
        currentPlan = goapPlanner.ComputePlan(startingNode, currentGoal);
        currentAction = GetNextAction();
    }

    private Action GetNextAction()
    {
        if (currentPlan != null && currentPlan.Count > 0)
        {
            Action action = currentPlan.Dequeue().Action;
            actionText.SetText(action.ActionName);
            return action;
        }

        return null;
    }

    private void CheckForNewGoal()
    {
        //Switch goal
        Goal BestGoal = goapPlanner.GetBestGoal();
        if (BestGoal != currentGoal || !currentAction)
        {
            currentGoal = BestGoal;
            goalText.SetText(currentGoal.name);
            if (currentAction)
                currentAction.Abort();
            ComputeNewPlan();
        }
    }
}
