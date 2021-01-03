using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class DragResize : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ResizeObject resizeComponent;
    private CheckValidObject checkComponent;


    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log("Drag Begin");
        resizeComponent.OnSelected();
        checkComponent.OnSelected();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("Dragging");
        Vector3 oldPoint = this.transform.position;
        Vector3 transPosInScreen = Camera.main.WorldToScreenPoint(this.transform.position);
        Vector3 newPoint = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, transPosInScreen.z));
        
        resizeComponent.OnDrag(newPoint, oldPoint);
        checkComponent.UpdateValid();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log("Drag Ended");
        resizeComponent.OnDeselected();
        checkComponent.OnDeselect();
    }

    // Start is called before the first frame update
    void Start()
    {
        resizeComponent = GameObject.Find("Interaction").GetComponent<ResizeObject>();
        checkComponent = GameObject.Find("Interaction").GetComponent<CheckValidObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
