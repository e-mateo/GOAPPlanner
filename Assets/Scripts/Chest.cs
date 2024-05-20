using UnityEngine;

public class Chest : MonoBehaviour
{
    int IronNB;
    private void Start()
    {
        World.Instance.RegisterChest(this);
    }

    private void OnDestroy()
    {
        World.Instance.UnregisterChest(this);
    }

    public void DepositIron()
    {
        IronNB++;
    }
}
