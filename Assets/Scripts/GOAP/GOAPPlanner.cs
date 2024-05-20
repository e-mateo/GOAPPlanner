using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GOAPPlanner : MonoBehaviour
{
    [Header("Debugs")]
    [SerializeField] bool bShowDebugLeaves;
    [SerializeField] bool bShowDebugPlan;
    [SerializeField] bool bShowDebugTimerCreatePlan;

    [Header("Planner")]
    public bool bDoBackwardResearch;
    [SerializeField] public WorldState startingWorldState;
    [SerializeField] List<Action> ActionsPool;
    [SerializeField] List<Goal> GoalPool;

    GraphBuilder graphBuilder = new GraphBuilder();
    Agent agent;
    Stopwatch stopwatch = new Stopwatch();

    private void Start()
    {
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        //instantiate Scriptable Object (used to trigger Awake and be able to edit them in play time without editing the asset)
        startingWorldState = Instantiate<WorldState>(startingWorldState);

        agent = GetComponent<Agent>();
        for (int i = 0; i < ActionsPool.Count; i++)
        {
            ActionsPool[i] = Instantiate<Action>(ActionsPool[i]);
            ActionsPool[i].SetAgent(agent);
        }

        for (int i = 0; i < GoalPool.Count; i++)
            GoalPool[i] = Instantiate<Goal>(GoalPool[i]);
    }

    public Queue<Node> ComputePlan(Node node, Goal currentGoal)
    {
        List<Node> leaves = new List<Node>();

        stopwatch.Reset();
        stopwatch.Start();
        if(bDoBackwardResearch)
        {
            Node goalNode = new Node(null, currentGoal.GoalWorldState, null);
            graphBuilder.BuildGraphBackward(goalNode, leaves, ActionsPool, node.State);
        }
        else
        {
            graphBuilder.BuildGraph(node, leaves, ActionsPool, currentGoal.GoalWorldState);
        }
        stopwatch.Stop();

        Queue<Node> plan = graphBuilder.CreatePlan(leaves, bDoBackwardResearch);

        ShowDebug(leaves, plan);

        return plan;
    }

    public Goal GetBestGoal()
    {
        Goal bestGoal = null;
        float bestPriority = 0;

        foreach(Goal goal in GoalPool)
        {
            if (!bestGoal || goal.GetPriority(agent) > bestPriority)
            {
                bestGoal = goal;
                bestPriority = bestGoal.GetPriority(agent);
            }
        }

        return bestGoal;
    }

    #region Debug
    private void ShowDebug(List<Node> leaves, Queue<Node> plan)
    {
        if (bShowDebugTimerCreatePlan)
            UnityEngine.Debug.Log("Time to create the plan : " + stopwatch.Elapsed.TotalMilliseconds / 1000f + " seconds");
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
            Queue<Node> plan = graphBuilder.CreatePlan(leaveList, bDoBackwardResearch);
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
