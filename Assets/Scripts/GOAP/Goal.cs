using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "GOAP/Goal/BaseGoal", order = 0)]
public class Goal : ScriptableObject
{
    [SerializeField] WorldState goalWorldStateSO;
    [SerializeField] Condition[] effectsPostGoal;

    Dictionary<StateType, bool> d_effects;
    WorldState goalWorldState;
    public WorldState GoalWorldState { get { return goalWorldState; } }

    public void Awake()
    {
        goalWorldState = Instantiate(goalWorldStateSO);

        d_effects = new Dictionary<StateType, bool>();

        foreach (Condition effect in effectsPostGoal)
            d_effects.Add(effect.name, effect.value);
    }

    public void PostValidateGoal(WorldState currentWorldState)
    {
        foreach (KeyValuePair<StateType, bool> effect in d_effects)
        {
            if (currentWorldState.D_WorldState.ContainsKey(effect.Key))
            {
                currentWorldState.D_WorldState[effect.Key] = effect.Value;
            }
        }
    }

    public virtual float GetPriority(Agent agent)
    {
        return 1;
    }
}
