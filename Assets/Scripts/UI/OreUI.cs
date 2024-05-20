using System.Collections.Generic;
using UnityEngine;

public class OreUI : MonoBehaviour
{

    List<UnityEngine.UI.Image> images = new List<UnityEngine.UI.Image>();

    // Start is called before the first frame update
    void Start()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        for(int i = 0; i < canvas.transform.childCount; i++)
            images.Add(canvas.transform.GetChild(i).GetComponent<UnityEngine.UI.Image>());
    }

    public void TakeOre(int amount)
    {
        int removeImage = 0;

        for(int i = 0; i < amount; i++)
        {
            if (images[i].gameObject.activeInHierarchy)
            {
                images[i].gameObject.SetActive(false);
                removeImage++;
                if (removeImage == amount)
                    return;
            }
        }
    }


}
