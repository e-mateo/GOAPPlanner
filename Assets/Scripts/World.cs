using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    private static World instance;
    public static World Instance { get { return instance; } }

    private void Awake()
    {
        instance = this;
    }

    public List<OreChunk> OreChunks { get; private set; } = new List<OreChunk>();
    public List<Furnace> Furnaces { get; private set; } = new List<Furnace>();
    public List<Chest> Chests { get; private set; } = new List<Chest>();
    public List<Agent> Agents { get; private set; } = new List<Agent>();

    private void Update()
    {
        UpdateGlobalWorldState();
    }
    void UpdateGlobalWorldState()
    {
        bool IronReady = GetFurnacesWithIron().Count > 0;

        foreach (Agent agent in Agents)
        {
            agent.GetWorldState().D_WorldState[StateType.IRONREADY] = IronReady;
        }
    }

    public void RegisterAgent(Agent agent)
    {
        Agents.Add(agent);
    }

    public void RegisterOre(OreChunk ore)
    {
        if (!OreChunks.Contains(ore))
        {
            OreChunks.Add(ore);
        }
    }

    public void UnregisterOre(OreChunk ore)
    {
        if (OreChunks.Contains(ore))
        {
            OreChunks.Remove(ore);
        }
    }

    public void RegisterFurnace(Furnace furnace)
    {
        if (!Furnaces.Contains(furnace))
        {
            Furnaces.Add(furnace);
        }
    }

    public void UnregisterFurnace(Furnace furnace)
    {
        if (Furnaces.Contains(furnace))
        {
            Furnaces.Remove(furnace);
        }
    }

    public void RegisterChest(Chest chest)
    {
        if (!Chests.Contains(chest))
        {
            Chests.Add(chest);
        }
    }

    public void UnregisterChest(Chest chest)
    {
        if (Chests.Contains(chest))
        {
            Chests.Remove(chest);
        }
    }

    public List<Furnace> GetAvailableFurnaces(int oreAmount)
    {
        List<Furnace> availableFurnaces = new List<Furnace>();
        foreach (Furnace furnace in Furnaces)
        {
            if (furnace.CanCraft(oreAmount))
                availableFurnaces.Add(furnace);
        }
        return availableFurnaces;
    }

    public List<Furnace> GetFurnacesWithIron()
    {
        List<Furnace> availableFurnaces = new List<Furnace>();
        foreach (Furnace furnace in Furnaces)
        {
            if (furnace.CanPickUp())
                availableFurnaces.Add(furnace);
        }
        return availableFurnaces;
    }
}
