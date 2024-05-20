using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EX1PlanGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    GOAPPlanner planner;

    void Start()
    {
        planner = GetComponent<GOAPPlanner>();
        Node startingNode = new Node(null, planner.startingWorldState, null);
        Goal goal = planner.GetBestGoal();
        Queue<Node> plan = planner.ComputePlan(startingNode, goal);
    }
}
