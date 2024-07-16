using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GOAPPlanner : MonoBehaviour //Component used to create a plan
{
    [Header("Debugs")]
    [SerializeField] bool bShowDebugLeaves;
    [SerializeField] bool bShowDebugPlan;
    [SerializeField] bool bShowDebugTimerCreatePlan;

    [Header("Planner")]
    [SerializeField] public WorldState startingWorldState;
    [SerializeField] List<Action> actionsPool; //All possible actions that the agent can make
    [SerializeField] List<Goal> goalPool; //All possible actions that the agent can reach

    Agent agent;
    GraphBuilder graphBuilder = new GraphBuilder();

    private void Start()
    {
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);

        //instantiate Scriptable Object (used to trigger Awake and be able to edit them in play time without editing the asset)
        startingWorldState = Instantiate<WorldState>(startingWorldState);

        agent = GetComponent<Agent>();
        for (int i = 0; i < actionsPool.Count; i++)
        {
            actionsPool[i] = Instantiate<Action>(actionsPool[i]);
            actionsPool[i].SetAgent(agent);
        }

        for (int i = 0; i < goalPool.Count; i++)
        {
            goalPool[i] = Instantiate<Goal>(goalPool[i]);
        }
    }

    public Queue<Node> ComputePlan(Node root, Goal currentGoal)
    {
        List<Node> leaves = new List<Node>();
        graphBuilder.BuildGraph(root, leaves, actionsPool, currentGoal.GoalWorldState); //Create every possible plan that will complete the goal

        Queue<Node> plan = graphBuilder.CreatePlan(leaves); //Choose and create the best plan (the ones with the cheapest cost)

        ShowDebug(leaves, plan);

        return plan;
    }

    public Goal GetBestGoal()
    {
        Goal bestGoal = null;
        float bestPriority = 0;

        foreach (Goal goal in goalPool)
        {
            float goalPriority = goal.GetPriority(agent);
            if (!bestGoal || goalPriority > bestPriority)
            {
                bestGoal = goal;
                bestPriority = goalPriority;
            }
        }

        return bestGoal;
    }

    #region Debug
    private void ShowDebug(List<Node> leaves, Queue<Node> plan)
    {
        if (leaves.Count > 0 && bShowDebugLeaves)
            DebugConsoleLeaves(leaves);
        if (plan != null && bShowDebugPlan)
            DebugConsolePlan(plan);
    }
    private void DebugConsoleLeaves(List<Node> leaves)
    {
        UnityEngine.Debug.Log("Leaves: \n------");
        foreach (Node leave in leaves)
        {
            List<Node> leaveList = new List<Node>();
            leaveList.Add(leave);
            Queue<Node> plan = graphBuilder.CreatePlan(leaveList);
            string planString = "";
            foreach (Node action in plan)
            {
                planString += action.Action.ActionName + " --> ";
            }
            UnityEngine.Debug.Log(planString + "cost = " + leave.Cost);
        }
    }

    private void DebugConsolePlan(Queue<Node> plan)
    {
        UnityEngine.Debug.Log("Plan taken: \n------");
        string planString = "";
        foreach (Node action in plan)
        {
            planString += action.Action.ActionName + " --> ";
        }
        planString += "Final State";
        UnityEngine.Debug.Log(planString);
    }

    #endregion
}
