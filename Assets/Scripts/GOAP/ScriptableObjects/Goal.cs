using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "GOAP/Goal/BaseGoal", order = 0)]
public class Goal : ScriptableObject
{
    [SerializeField] WorldState goalWorldStateSO; //The world state that needs to be reached to complete the goal
    [SerializeField] Condition[] effectsPostGoal; //The effects to apply to the world state once the goal is completed

    Dictionary<StateType, bool> d_effects;
    public WorldState GoalWorldState { get { return goalWorldStateSO; } }

    public void Awake()
    {
        goalWorldStateSO = Instantiate(goalWorldStateSO);

        //Copy the effectsPostGoal in the effect dictionnary
        d_effects = new Dictionary<StateType, bool>();

        foreach (Condition effect in effectsPostGoal)
            d_effects.Add(effect.state, effect.value);
    }

    public void PostValidateGoal(WorldState currentWorldState)
    {
        foreach (KeyValuePair<StateType, bool> effect in d_effects) //Apply each effect to the current world state
        {
            currentWorldState.UpdateValue(effect.Key, effect.Value);
        }
    }

    public virtual float GetPriority(Agent agent) //Get the priority of the goal (the goal with the higher priority will be the current goal)
    {
        return 1;
    }
}
