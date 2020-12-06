using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    private GameObject targetGameObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {
        targetGameObject = GameObject.Find("GameManager").GetComponent<ObjectManager>().targetGameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
