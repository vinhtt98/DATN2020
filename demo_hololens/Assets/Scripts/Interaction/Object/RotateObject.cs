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

    public void OnSelected(Vector3 startPoint) {
        ObjectManager objMngr = GameObject.Find("GameManager").GetComponent<ObjectManager>();
        targetGameObject = objMngr.targetGameObject;
        isOnVerticalPlane = objMngr.isOnVerticalPlane;
        startValue = targetGameObject.transform.localEulerAngles;
        this.startPoint = startPoint;
    }

    // Update is called once per frame
    public void UpdateRotation(Vector2 pos)
    {
        float delta = calcLength(pos) * rotateSpeed;
        Vector3 newValue = startValue;
        if (isOnVerticalPlane)
            newValue.z = newValue.z + delta;
        else
            newValue.y = newValue.y + delta;
        targetGameObject.transform.localEulerAngles = newValue;
    }

    private float calcLength(Vector2 pos) {
        Vector3 endPoint = pos;
        float direction = -1;
        if (endPoint.x < startPoint.x)
            direction = 1;
        return direction * Mathf.Sqrt((endPoint.x - startPoint.x)*(endPoint.x - startPoint.x) + (endPoint.y - startPoint.y)*(endPoint.y - startPoint.y));
    }
}
