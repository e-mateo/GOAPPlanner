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
}


[System.Serializable]
[CreateAssetMenu(menuName = "GOAP/WorldState", order = 0)]
public class WorldState : ScriptableObject //The WorldState is represented with a dictionnary of StateType enum and Booleans, each AI has its own world state
                                           //It is often made with a dictionnary of string and bool but i decided to use a enum to avoid string comparison
{
    [SerializeField] State[] worldState; //Used to be editable in the inspector
    Dictionary<StateType, bool> d_WorldState = new Dictionary<StateType, bool>();

    public Dictionary<StateType, bool> D_WorldState {  get { return d_WorldState; } }

    private void Awake()
    {
        //Copy world state arrar into d_WorldState dictionnary
        d_WorldState = new Dictionary<StateType, bool>();

        if (worldState != null)
        {
            foreach (State state in worldState)
                d_WorldState.Add(state.name, state.value);
        }
    }

    public void Copy(WorldState copy)
    {
        foreach (KeyValuePair<StateType, bool> state in copy.D_WorldState)
        {
            d_WorldState.Add(state.Key, state.Value);
        }
    }

    public void UpdateValue(StateType type, bool value)
    {
        if(d_WorldState.ContainsKey(type))
            d_WorldState[type] = value;
    }

    public bool GetValue(StateType type)
    {
        return d_WorldState[type];
    }

    public bool TryGetValue(StateType type, out bool value)
    {
        return D_WorldState.TryGetValue(type, out value);
    }
}
