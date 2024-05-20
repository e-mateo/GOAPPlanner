using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    Canvas canva;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        canva = GetComponent<Canvas>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        canva.transform.rotation = cam.transform.rotation;
    }
}
