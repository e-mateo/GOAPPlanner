using UnityEngine;

public class FaceCamera : MonoBehaviour //Component used to make a canva faces the camera
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
