using System.Collections.Generic;
using UnityEngine;

public class GraphBuilder
{
    //Used to create every possible plans in a tree graph
    public void BuildGraph(Node parent, List<Node> outLeaves, List<Action> availableActions, WorldState goal)
    {
        foreach(Action action in availableActions) //Loop through each available Action
        {
            if(action.IsValid(parent.State)) //Check action preconditions
            {
                WorldState newWorldState = action.ApplyEffect(parent.State);
                Node nextNode = new Node(parent, newWorldState, action); //Create the next Node of the branch
                if (IsGoalAchieved(newWorldState, goal)) //check if the action completes the goal
                {
                    outLeaves.Add(nextNode); //Add the node to the leaves (leaves are the final nodes that validates the goal)
                }
                else
                {
                    List<Action> actionsSubSet = CreateActionSubSet(availableActions, action); //Remove the action from the available action
                    BuildGraph(nextNode, outLeaves, actionsSubSet, goal); //Recursive function to select the next action until the goal is completed or that no more action are available
                }
            }
        }
    }

    //Create the final plan
    public Queue<Node> CreatePlan(List<Node> leaves /*leaves are the final nodes that validates the goal*/)
    {
        //Choose lowest plan
        if (leaves == null || leaves.Count == 0)
            return null;

        Node cheapestLeave = GetCheapestLeaves(leaves);

        //Create the plan
        List<Node> plan = new List<Node>();
        Node currentNode = cheapestLeave;
        while (currentNode.Parent != null) //Go back from the leave node to the root node to create the plan
        {
            plan.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        plan.Reverse(); //Reverse the plan to start from the root to the leave

        Queue<Node> planQueue = new Queue<Node>(); //Copy the plan into a queue
        foreach (Node node in plan)
            planQueue.Enqueue(node);

        return planQueue;
    }

    Node GetCheapestLeaves(List<Node> leaves)
    {
        Node cheapestLeave = null;
        foreach (Node leave in leaves)
        {
            if (cheapestLeave == null || leave.Cost < cheapestLeave.Cost)
            {
                cheapestLeave = leave;
            }
        }
        return cheapestLeave;
    }

    bool IsGoalAchieved(WorldState worldState, WorldState goal)
    {
        if (!goal) return false;

        foreach(KeyValuePair<StateType, bool> goalState in goal.D_WorldState) //Check if the goal worldstate condition matches the current worldstate
        {
            bool stateValue;
            if (worldState.TryGetValue(goalState.Key, out stateValue) && stateValue == goalState.Value) //Check if the value of the state is the same in the worldState and the goalState
            {
                continue;
            }

            return false;
        }

        return true;
    }

    List<Action> CreateActionSubSet(List<Action> availableActions, Action action)
    {
        List<Action> subsetAction = new List<Action>(availableActions);
        subsetAction.Remove(action);
        return subsetAction;
    }
}
