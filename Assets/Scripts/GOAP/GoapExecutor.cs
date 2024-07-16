using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoapExecutor : MonoBehaviour //Component used to execute the Goap Planner in real time
{
    GOAPPlanner goapPlanner;
    Queue<Node> currentPlan;
    Goal currentGoal;
    Action currentAction;
    Agent agent;


    private void Start()
    {
        goapPlanner = GetComponent<GOAPPlanner>();
        agent = GetComponent<Agent>();
    }

    private void Update()
    {
        SelectGoal(); //Select the current goal
        DoCurrentAction();
    }

    private void SelectGoal()
    {
        Goal bestGoal = goapPlanner.GetBestGoal();

        if (bestGoal != currentGoal || !currentAction)
        {
            SwitchGoal(bestGoal);
        }
    }

    void SwitchGoal(Goal newGoal)
    {
        //If the goal changes, make a new plan
        currentGoal = newGoal;
        ComputeNewPlan();
    }

    void DoCurrentAction()
    {
        if (!currentAction) return;

        if (!currentAction.IsValid(agent.GetWorldState())) //Check if the precondition and procedural precondition are still valid
        {
            ComputeNewPlan();
            return;
        }

        bool completed = currentAction.ExecuteAction();
        if (completed)
        {
            CompleteAction();
        }
    }

    private void CompleteAction()
    {
        WorldState WorldStatePostAction = currentAction.ApplyEffect(agent.GetWorldState()); //Apply the effects of the action on the agent world state
        agent.SetWorldState(WorldStatePostAction);

        currentAction.EndAction();
        currentAction = GetNextAction();
        currentAction?.StartAction();

        if (currentAction == null) //Has Reached the end of the plan: the goal is completed
        {
            currentGoal.PostValidateGoal(agent.GetWorldState());
            SelectGoal(); //Select a new goal
        }
    }

    private void ComputeNewPlan()
    {
        currentAction?.Abort();
        Node startingNode = new Node(null, agent.GetWorldState(), null);
        currentPlan = goapPlanner.ComputePlan(startingNode, currentGoal);
        currentAction = GetNextAction();
        currentAction?.StartAction();
    }

    private Action GetNextAction()
    {
        if (currentPlan != null && currentPlan.Count > 0)
        {
            Action action = currentPlan.Dequeue().Action;
            return action;
        }

        return null;
    }
}
