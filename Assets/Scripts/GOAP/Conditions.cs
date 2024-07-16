
[System.Serializable]
public class Condition //A Condition is used if a state in the world state matches the value of the condition
{
    public StateType state;
    public bool value;
}

[System.Serializable]
public class ConditionnalCost : Condition //A ConditionnalCost overrides the base cost of the action if the world state matches the condition
{
    public int overrideCost;
}
