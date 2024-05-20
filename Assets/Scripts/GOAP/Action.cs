using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Condition
{
    public StateType name;
    public bool value;
}

[System.Serializable]
public class ConditionnalCost
{
    public StateType name;
    public bool value;
    public int cost;
}

[CreateAssetMenu(menuName = "GOAP/Action/BaseAction", order = 0)]
public class Action : ScriptableObject
{
    [SerializeField] string actionName;
    [SerializeField] int baseCost;
    [SerializeField] List<ConditionnalCost> conditionnalCosts;

    [SerializeField] Condition[] preconditions;
    [SerializeField] Condition[] effects;

    Dictionary<StateType, bool> d_preconditions;
    Dictionary<StateType, bool> d_effects;

    GameObject target;
    protected GameObject Target {  get { return target; } set { target = value; agent.CurrentTarget = value; } }

    bool isRunning;
    protected Agent agent;
    public string ActionName { get { return actionName; } }
    public bool IsRunning { get { return isRunning; } set { isRunning = value; } }

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

    public void Awake()
    {
        d_preconditions = new Dictionary<StateType, bool>();
        d_effects = new Dictionary<StateType, bool>();

        //Copy Array in Dictionnaries (Array are used for the editor)
        foreach (Condition condition in preconditions)
            d_preconditions.Add(condition.name, condition.value);

        foreach (Condition effect in effects)
            d_effects.Add(effect.name, effect.value);
    }

    public void SetAgent(Agent agent) {  this.agent = agent; }

    public int GetCost(WorldState worldState)
    {
        foreach(ConditionnalCost condCost in conditionnalCosts)
        {
            if (worldState.D_WorldState.ContainsKey(condCost.name) && worldState.D_WorldState[condCost.name] == condCost.value)
                return condCost.cost;
        }
        return baseCost;
    }

    public bool IsValid(WorldState parentState)
    {
        foreach (KeyValuePair<StateType, bool> condition in d_preconditions)
        {
            bool value;
            if (parentState.D_WorldState.TryGetValue(condition.Key, out value))
            {
                if (value == condition.Value)
                    continue;
            }

            return false;
        }

        if (!ProceduralPrecondition())
            return false;

        return true;
    }

    public WorldState ApplyEffect(WorldState parentState)
    {
        WorldState newWorldState = CreateInstance<WorldState>();
        newWorldState.Copy(parentState);

        foreach (KeyValuePair<StateType, bool> effect in d_effects)
        {
            if (newWorldState.D_WorldState.ContainsKey(effect.Key))
            {
                newWorldState.D_WorldState[effect.Key] = effect.Value;
            }
        }

        return newWorldState;
    }

    public bool BackwardValidateEffect(WorldState parentState)
    {
        foreach (KeyValuePair<StateType, bool> effect in d_effects)
        {
            bool value;
            if (parentState.D_WorldState.TryGetValue(effect.Key, out value))
            {
                if (value != effect.Value)
                    return false;
            }
        }

        return true;
    }

    public WorldState BackwardValidateAndApplyCondition(WorldState parentState)
    {
        WorldState newWorldState = CreateInstance<WorldState>();
        newWorldState.Copy(parentState);

        foreach (KeyValuePair<StateType, bool> condition in d_preconditions)
        {
            bool value;
            if (parentState.D_WorldState.TryGetValue(condition.Key, out value))
            {
                if (value == condition.Value)
                    continue;
                else if (d_effects.ContainsKey(condition.Key))
                    newWorldState.D_WorldState[condition.Key] = condition.Value;
                else
                    return null;
            }
            else
            {
                newWorldState.D_WorldState.Add(condition.Key, condition.Value);
            }
        }

        if (!ProceduralPrecondition())
            return null;

        return newWorldState;
    }


    public virtual bool ProceduralPrecondition() // Called when checking IsValid
    {
        return true;
    }

    public virtual void StartAction() //Called at the beginning of the action
    { 
        if(HasTarget())
            GoToTarget();

        isRunning = true;
    }

    public virtual bool ExecuteAction() //Called each frame when IsRunning
    { 
        return true; 
    }

    public virtual void EndAction() //Called after ApplyEffect
    { 
        isRunning = false;
    }

    public virtual void Abort() //Called when ShouldAbort is true
    {
        isRunning = false;
    }
}
