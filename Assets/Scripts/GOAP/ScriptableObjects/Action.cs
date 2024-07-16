using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/Action/BaseAction", order = 0)]
public class Action : ScriptableObject
{
    [SerializeField] string actionName;
    [SerializeField] int baseCost; //The cost of the action
    [SerializeField] List<ConditionnalCost> conditionnalCosts; //conditional costs override the base cost of the action if the world state matches their conditions

    [SerializeField] Condition[] preconditions; //The preconditions that the world state needs to enter this action (used to be editable in the inspector)
    [SerializeField] Condition[] effects; //Effects will update the world state values after finishing the action (used to be editable in the inspector)
    Dictionary<StateType, bool> d_preconditions; //Store preconditions as dictionnary (can't edit dictionnary in the inspector)
    Dictionary<StateType, bool> d_effects; //Store effects as dictionnary (can't edit dictionnary in the inspector)

    protected bool isRunning; //Tell if the action is currently running 
    protected Agent agent; //Ref to the AI executing the action
    GameObject target; //The target gameobject to reach to execute this action

    public bool IsRunning { get { return isRunning; } set { isRunning = value; } }
    public string ActionName { get { return actionName; } }
    protected GameObject Target { get { return target; } set { target = value; agent.CurrentTarget = value; } }
    public void SetAgent(Agent agent) { this.agent = agent; }


    public void Awake()
    {
        d_preconditions = new Dictionary<StateType, bool>();
        d_effects = new Dictionary<StateType, bool>();

        //Copy Array in Dictionnaries (Array are used for the editor because we can't edit dictionnary in the inspector)
        foreach (Condition condition in preconditions)
            d_preconditions.Add(condition.state, condition.value);

        foreach (Condition effect in effects)
            d_effects.Add(effect.state, effect.value);
    }

    public int GetActionCost(WorldState worldState)
    {
        //Check if a conditionnal cost is valid
        foreach(ConditionnalCost condCost in conditionnalCosts)
        {
            bool value;
            if (worldState.TryGetValue(condCost.state, out value) && value == condCost.value)
            {
                return condCost.overrideCost;
            }
        }

        //if there are no valid conditionnal costs, returns the base cost of the action
        return baseCost;
    }

    public bool IsValid(WorldState worldState) //Check if the preconditions are valid to execute the action
    {
        foreach (KeyValuePair<StateType, bool> condition in d_preconditions) //check every preconditions
        {
            bool stateValue;
            if (worldState.TryGetValue(condition.Key, out stateValue) && stateValue == condition.Value)
            {
                continue;
            }

            //If one condition is not met, the action isn't valid
            return false;
        }

        if (!ProceduralPrecondition()) //check if the procedural precondition is valid (a precondition that is pre-coded)
            return false;

        return true;
    }

    public WorldState ApplyEffect(WorldState worldState) //Apply the effect of the Action to the world state once the action is finished
    {
        WorldState newWorldState = CreateInstance<WorldState>();
        newWorldState.Copy(worldState);

        foreach (KeyValuePair<StateType, bool> effect in d_effects) //loop through each effect and try to apply them
        {
            newWorldState.UpdateValue(effect.Key, effect.Value);
        }

        return newWorldState;
    }

    #region VirtualActionFunctions
    public virtual bool ProceduralPrecondition() // Called when checking IsValid, can be overrided for each Action
    {
        return true;
    }

    public virtual void StartAction() //Called at the beginning of the action, can be overrided for each Action
    { 
        if(HasTarget())
            GoToTarget();

        isRunning = true;
    }

    public virtual bool ExecuteAction() //Called each frame when the action is running, can be overrided for each Action
    {                                   //Returns if the action succeded
        return true; 
    }

    public virtual void EndAction() //Called after ApplyEffect, can be overrided for each Action
    { 
        isRunning = false;
    }

    public virtual void Abort() //Called when the action is aborted, can be overrided for each Action
    {
        isRunning = false;
    }
    #endregion

    #region Target
    public void GoToTarget()
    {
        agent.NavMeshAgent.destination = target.transform.position;
    }

    public bool IsAtTarget()
    {
        return agent.NavMeshAgent.remainingDistance < 2f;
    }

    public bool HasTarget()
    {
        return target != null;
    }
    #endregion

}
