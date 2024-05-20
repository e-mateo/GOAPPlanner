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

    public void ActivateIronUI(bool activate)
    {
        iron.gameObject.SetActive(activate);
    }

    public void UpdateOreUI(int currentOre)
    {
        if(currentOre == 2)
        {
            ore1.sprite = ironFull;
            ore2.sprite = ironFull;
        }
        else if(currentOre == 1) 
        {
            ore1.sprite = ironFull;
            ore2.sprite = ironEmpty;
        }
        else
        {
            ore1.sprite = ironEmpty;
            ore2.sprite = ironEmpty;
        }
    }

}
