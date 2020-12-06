using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject targetGameObject;
    private MoveObject moveComponent;
    private RotateObject rotateComponent;
    private ResizeObject resizeComponent;
    public bool isCanResize;
    public bool isOnVerticalPlane;
    public bool isOnCeil;
    // Start is called before the first frame update
    void Start()
    {
        moveComponent = GameObject.Find("Interaction").GetComponent<MoveObject>();
        rotateComponent = GameObject.Find("Interaction").GetComponent<RotateObject>();
        resizeComponent = GameObject.Find("Interaction").GetComponent<ResizeObject>();

        moveComponent.enabled = false;
        rotateComponent.enabled = false;
        resizeComponent.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            onSelectRotate();
        }
    }

    private void onSelectRotate() {
        rotateComponent.enabled = true;
        rotateComponent.startPoint = Input.GetTouch(0).position;
    }

    public void onSelectResize() {
        resizeComponent.enabled = !resizeComponent.enabled;
    }
}
