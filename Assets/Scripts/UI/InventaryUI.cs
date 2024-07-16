using UnityEngine;
using UnityEngine.UI;

public class InventaryUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Image ore1;
    [SerializeField] Image ore2;
    [SerializeField] Image iron;

    [SerializeField] Sprite ironEmpty;
    [SerializeField] Sprite ironFull;


    private void Start()
    {
        iron.gameObject.SetActive(false);
    }

    public void ActivateIronUI(bool active)
    {
        iron.gameObject.SetActive(active);
    }

    public void UpdateOreUI(int currentOre)
    {
        ore1.sprite = currentOre >= 1 ? ironFull : ironEmpty;
        ore2.sprite = currentOre >= 2 ? ironFull : ironEmpty;
    }

}
