using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragRotate : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RotateObject rotateComponent;
    private CheckValidObject checkComponent;
    //Implement handler
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log("Drag Begin");
        rotateComponent.OnSelected(eventData.position);
        checkComponent.OnSelected();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("Dragging");
        rotateComponent.UpdateRotation(eventData.position);
        checkComponent.UpdateValid();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log("Drag Ended");
        checkComponent.OnDeselect();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        rotateComponent = GameObject.Find("Interaction").GetComponent<RotateObject>();
        checkComponent = GameObject.Find("Interaction").GetComponent<CheckValidObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
