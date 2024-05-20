using System.Collections.Generic;
using UnityEngine;

public class GraphBuilder
{
    public void BuildGraph(Node parent, List<Node> leaves, List<Action> availableActions, WorldState goal)
    {
        foreach(Action action in availableActions)
        {
            if(action.IsValid(parent.State))
            {
                WorldState newWorldState = action.ApplyEffect(parent.State);
                Node node = new Node(parent, newWorldState, action);
                if (GoalAchieved(newWorldState, goal))
                {
                    leaves.Add(node);
                }
                else
                {
                    List<Action> actionsSubSet = CreateActionSubSet(availableActions, action);
                    BuildGraph(node, leaves, actionsSubSet, goal);
                }
            }
        }
    }

    public void BuildGraphBackward(Node parent, List<Node> leaves, List<Action> availableActions, WorldState start)
    {
        foreach (Action action in availableActions)
        {
            if (action.BackwardValidateEffect(parent.State))
            {
                WorldState BeforeAction = action.BackwardValidateAndApplyCondition(parent.State);

                if (BeforeAction)
                {
                    if (parent.Parent == null)
                    {
                        if (GoalAchieved(BeforeAction, parent.State)) //Can't be the first action because the world state before the action still validates the goal
                            continue;
                    }

                    Node node = new Node(parent, BeforeAction, action);
                    if (StartAchieved(BeforeAction, start))
                    {
                        leaves.Add(node);
                    }
                    else
                    {
                        List<Action> actionsSubSet = CreateActionSubSet(availableActions, action);
                        BuildGraphBackward(node, leaves, actionsSubSet, start);
                    }
                }
            }
        }
    }


    public Queue<Node> CreatePlan(List<Node> leaves, bool backward)
    {
        //Choose lowest plan
        if (leaves == null || leaves.Count == 0)
            return null;

        Node lowestCostLeave = null;
        foreach (Node node in leaves)
        {
            if(lowestCostLeave == null)
            {
                lowestCostLeave = node;
            }
            else if(node.Cost < lowestCostLeave.Cost)
            {
                lowestCostLeave = node;
            }
        }

        //Create the plan
        List<Node> plan = new List<Node>();
        Node currentNode = lowestCostLeave;
        while (currentNode.Parent != null)
        {
            plan.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        if(!backward)
            plan.Reverse();

        Queue<Node> planQueue = new Queue<Node>();
        foreach (Node node in plan)
            planQueue.Enqueue(node);

        return planQueue;
    }
    private bool GoalAchieved(WorldState newState, WorldState goal)
    {
        if (!goal)
            return false;

        foreach(KeyValuePair<StateType, bool> state in goal.D_WorldState)
        {
            bool value;
            if (newState.D_WorldState.TryGetValue(state.Key, out value))
            {
                if (value == state.Value)
                    continue;
            }

            return false;
        }

        return true;
    }

    private bool StartAchieved(WorldState currentWorldState, WorldState startingWorldState)
    {
        foreach (KeyValuePair<StateType, bool> state in startingWorldState.D_WorldState)
        {
            bool value;
            if (currentWorldState.D_WorldState.TryGetValue(state.Key, out value))
            {
                if (value != state.Value)
                    return false;
            }
        }

        return true;
    }

    private List<Action> CreateActionSubSet(List<Action> availableActions, Action action)
    {
        List<Action> subsetAction = new List<Action>(availableActions);
        subsetAction.Remove(action);
        return subsetAction;
    }


}
