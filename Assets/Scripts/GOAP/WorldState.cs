using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class State
{
    public StateType name;
    public bool value;
}

public enum StateType
{ 
    HASORE,
    HASIRON,
    USEDFURNACE,
    ADDEDBAR,
    IRONREADY,

    IS_ENEMY_DEAD,
    HAS_WEAPON,
    IS_NEAR_WEAPON,
    IS_ENEMY_NEAR,
    IS_HURT,
}


[System.Serializable]
[CreateAssetMenu(menuName = "GOAP/WorldState", order = 0)]
public class WorldState : ScriptableObject
{
    [SerializeField] State[] worldState;
    Dictionary<StateType, bool> d_worldState = new Dictionary<StateType, bool>();

    public Dictionary<StateType, bool> D_WorldState {  get { return d_worldState; } }

    private void Awake()
    {
        d_worldState = new Dictionary<StateType, bool>();

        if (worldState != null)
        {
            foreach (State state in worldState)
                d_worldState.Add(state.name, state.value);
        }
    }

    public void Copy(WorldState copy)
    {
        foreach (KeyValuePair<StateType, bool> state in copy.D_WorldState)
        {
            d_worldState.Add(state.Key, state.Value);
        }
    }

    public void UpdateValue(StateType type, bool value)
    {
        if(d_worldState.ContainsKey(type))
            d_worldState[type] = value;
    }

    public bool GetValue(StateType type)
    {
        return d_worldState[type];
    }
}
