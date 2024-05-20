using System.Collections;
using UnityEngine;

public class Furnace : MonoBehaviour
{
    public Transform CraftingVisual;
    public Transform IronVisual;
    public Transform ProgressBarRoot;
    public Transform ProgressBar;
    public static int OreCost = 2;
    public static int MaxQueued = 3;
    [SerializeField] private float _craftTime = 5;
    private int _craftQueue = 0;

    private int _barCrafted = 0;

    private float _craftTimer = 0.0f;
    private bool _isCrafting = false;

    private void Start()
    {
        World.Instance.RegisterFurnace(this);
    }

    private void OnDestroy()
    {
        World.Instance.UnregisterFurnace(this);
    }

    public bool CanCraft(int oreAmount = 2)
    {
        return oreAmount >= OreCost && _craftQueue < MaxQueued;
    }

    public int TryCraft(int oreAmount)
    {
        if (!CanCraft(oreAmount))
            return oreAmount;

        if (_isCrafting)
        {
            _craftQueue++;

            return oreAmount - OreCost;
        }

        StartCoroutine(Craft());
        return oreAmount - OreCost;
    }

    IEnumerator Craft()
    {
        CraftingVisual.gameObject.SetActive(true);
        ProgressBarRoot.gameObject.SetActive(true);

        _isCrafting = true;
        _craftTimer = 0;

        while (_isCrafting)
        {
            Vector3 progressScale = ProgressBar.localScale;
            progressScale.z = _craftTimer / _craftTime;
            ProgressBar.localScale = progressScale;

            if (_craftTimer < _craftTime)
            {
                _craftTimer += Time.deltaTime;
                yield return null;
                continue;
            }

            _barCrafted++;
            IronVisual.gameObject.SetActive(true);
            _craftTimer = 0;

            if (_craftQueue > 0)
            {
                _craftQueue--;
            }
            else
            {
                _isCrafting = false;
                CraftingVisual.gameObject.SetActive(false);
                ProgressBarRoot.gameObject.SetActive(false);
            }

            yield return null;
        }
    }

    public bool CanPickUp()
    {
        return _barCrafted > 0;
    }

    public bool TryPickUp()
    {
        if (CanPickUp())
        {
            _barCrafted--;

            if (_barCrafted == 0)
                IronVisual.gameObject.SetActive(false);

            return true;
        }
        return false;
    }


}
