using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

public class DragMove : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ObjectManager objectManager;
    private MoveObject moveComponent;
    private CheckValidObject checkComponent;

    private ARSessionOrigin arOrigin;
    private ARReferencePointManager arReferencePointManager;
    private Pose placementPose;
    private ARPlane placementPlane;
    private bool placementPoseIsValid = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log("Drag Begin");
        objectManager.setTargetGameObject(null);
        objectManager.setTargetGameObject(this.gameObject);
        if (this.gameObject.GetComponent<ARReferencePoint>() == null)
        {
            // this.gameObject.AddComponent<ARReferencePoint>();
        }
        moveComponent.OnSelected();
        checkComponent.OnSelected();
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdatePlacementPose(eventData.position);
        if (placementPoseIsValid)
        {
            Vector3 pos = placementPose.position;
            // pos.x += 0.01f;
            // pos.y += 0.01f;
            // pos.z += 0.01f;
            moveComponent.OnDrag(pos);
        }
        checkComponent.UpdateValid();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log("Drag Ended");
        moveComponent.OnDeselected();
        checkComponent.OnDeselect();
    }

    // Start is called before the first frame update
    void Start()
    {
        objectManager = GameObject.Find("GameManager").GetComponent<ObjectManager>();
        moveComponent = GameObject.Find("Interaction").GetComponent<MoveObject>();
        checkComponent = GameObject.Find("Interaction").GetComponent<CheckValidObject>();

        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arReferencePointManager = arOrigin.GetComponent<ARReferencePointManager>();
    }

    private void UpdatePlacementPose(Vector2 touchPos)
    {
        var hits = new List<ARRaycastHit>();
        arOrigin.Raycast(touchPos, hits, TrackableType.PlaneWithinBounds);

        placementPoseIsValid = hits.Count > 0;

        if (placementPoseIsValid)
        {
            ARPlane newPlacementPlane = arOrigin.GetComponent<ARPlaneManager>().TryGetPlane(hits[0].trackableId);

            PlaneAlignment align = newPlacementPlane.boundedPlane.Alignment;
            if (objectManager.isOnVerticalPlane)
            {
                placementPoseIsValid = align != PlaneAlignment.Horizontal;
            }
            if (objectManager.isOnCeil)
            {
                placementPoseIsValid = Camera.main.ScreenToWorldPoint(touchPos).y < placementPose.position.y;
            }

            if (!placementPoseIsValid)
                return;

            placementPose = hits[0].pose;
            placementPlane = newPlacementPlane;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}
