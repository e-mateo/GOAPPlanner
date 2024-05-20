using UnityEngine;

public class OreChunk : MonoBehaviour
{
    [SerializeField] private int _amount;
    public int Amount { get { return _amount; } }

    private int _reserved = 0;

    private OreUI oreUI;
    private void Start()
    {
        World.Instance.RegisterOre(this);
        oreUI = GetComponentInChildren<OreUI>();
    }

    private void OnDestroy()
    {
        World.Instance.UnregisterOre(this);
    }


    public bool HasOreAvailable()
    {
        return _amount > 0;
    }

    public bool ReserveOre(int amount = 1)
    {
        if (_amount - _reserved < amount)
            return false;

        if (HasOreAvailable())
        {
            _reserved += amount;
            _amount -= amount;
            return true;
        }
        return false;
    }

    public void UnReserveOre(int amount = 1)
    {
        if(_reserved >= amount)
        {
            _reserved -= amount;
            _amount += amount;
        }
    }

    public bool PickUpOre(int amount = 1)
    {
        if (amount > _reserved)
            return false;

        _reserved -= amount;
        oreUI.TakeOre(amount);

        if (_amount <= 0)
            Destroy(gameObject);

        return true;
    }
}
