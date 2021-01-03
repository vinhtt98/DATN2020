using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    private GameObject targetGameObject;

    public void OnSelected()
    {
        targetGameObject = GameObject.Find("GameManager").GetComponent<ObjectManager>().targetGameObject;
    }

    public void OnDeselected()
    {
    }

    public void OnDrag(Vector3 pos)
    {
        targetGameObject.transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
