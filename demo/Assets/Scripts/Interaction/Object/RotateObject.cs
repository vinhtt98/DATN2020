using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotateSpeed = 0.1f;
    private GameObject targetGameObject;
    private bool isOnVerticalPlane;
    public Vector3 startPoint;
    private Vector3 startValue;

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnEnable() {
        ObjectManager objMngr = GameObject.Find("GameManager").GetComponent<ObjectManager>();
        targetGameObject = objMngr.targetGameObject;
        isOnVerticalPlane = objMngr.isOnVerticalPlane;
        startValue = targetGameObject.transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
            this.enabled = false;
        }

        float delta = calcLength() * rotateSpeed;
        Vector3 newValue = startValue;
        if (isOnVerticalPlane)
            newValue.z = newValue.z + delta;
        else
            newValue.y = newValue.y + delta;
        targetGameObject.transform.localEulerAngles = newValue;
    }

    private float calcLength() {
        if (Input.touchCount > 0) {
            Vector3 endPoint = Input.GetTouch(0).position;
            float direction = -1;
            if (endPoint.x < startPoint.x)
                direction = 1;
            return direction * Mathf.Sqrt((endPoint.x - startPoint.x)*(endPoint.x - startPoint.x) + (endPoint.y - startPoint.y)*(endPoint.y - startPoint.y));
        }
        return 0;
    }
}
