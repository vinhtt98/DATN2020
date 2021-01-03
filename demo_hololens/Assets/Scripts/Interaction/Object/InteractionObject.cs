using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionObject : MonoBehaviour, IPointerDownHandler, IPointerClickHandler,
    IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ObjectManager objectManager;
    private MoveObject moveComponent;
    private RotateObject rotateComponent;
    private ResizeObject resizeComponent;
    private BoxTransform boxComponent;
    public float tapTimeLength;
    private float tapCountdown;
    //Implement handler
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log("Drag Begin");
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("Dragging");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log("Drag Ended");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
        if (objectManager.targetGameObject != this.gameObject)
        {
            objectManager.setTargetGameObject(null);
            objectManager.setTargetGameObject(this.gameObject);

            int id = int.Parse(this.gameObject.name);
            DeployObjectProperty property;
            if (ABUtils.goPropDict.TryGetValue(id, out property))
            {
                objectManager.objectSize = property.objectSize;
                objectManager.isCanResize = property.isCanResize;
                objectManager.isOnVerticalPlane = property.isOnVerticalPlane;
                objectManager.isOnCeil = property.isOnCeil;
            }
        }
        else
        {
            objectManager.setTargetGameObject(null);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("Mouse Down: " + eventData.pointerCurrentRaycast.gameObject.name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("Mouse Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("Mouse Exit");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("Mouse Up");
    }

    // Start is called before the first frame update
    void Start()
    {
        objectManager = GameObject.Find("GameManager").GetComponent<ObjectManager>();
        moveComponent = GameObject.Find("Interaction").GetComponent<MoveObject>();
        rotateComponent = GameObject.Find("Interaction").GetComponent<RotateObject>();
        resizeComponent = GameObject.Find("Interaction").GetComponent<ResizeObject>();
        boxComponent = GameObject.Find("Interaction").GetComponent<BoxTransform>();

        boxComponent.enabled = false;
    }
}
