using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class DragMove : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ObjectManager objectManager;
    private MoveObject moveComponent;
    private CheckValidObject checkComponent;

    private ARSessionOrigin arOrigin;
    private ARReferencePointManager arReferencePointManager;
    private ARRaycastManager arRaycastManager;

    private Pose placementPose;
    private ARPlane placementPlane;
    private bool placementPoseIsValid = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log("Drag Begin");
        objectManager.setTargetGameObject(null);
        objectManager.setTargetGameObject(this.gameObject);

        placementPlane = null;
        ARReferencePoint refPoint = this.gameObject.GetComponent<ARReferencePoint>();
        if (refPoint != null)
            Destroy(refPoint);

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
        if (placementPlane != null)
        {
            try
            {
                ARReferencePoint refPoint = arReferencePointManager.AttachReferencePoint(placementPlane, placementPose);
                this.transform.SetPositionAndRotation(refPoint.transform.position, refPoint.transform.rotation);
                Transform child = this.transform.GetChild(0);
                refPoint.transform.name = this.transform.name;
                refPoint.transform.localScale = this.transform.localScale;
                child.SetParent(refPoint.transform);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                if (this.gameObject.GetComponent<ARReferencePoint>() == null)
                {
                    this.gameObject.AddComponent<ARReferencePoint>();
                }
            }
        }
        else
        {
            this.gameObject.AddComponent<ARReferencePoint>();
        }

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
        arRaycastManager = arOrigin.GetComponent<ARRaycastManager>();
    }

    private void UpdatePlacementPose(Vector2 touchPos)
    {
        var hits = new List<ARRaycastHit>();
        arRaycastManager.Raycast(touchPos, hits, TrackableType.PlaneWithinBounds);

        placementPoseIsValid = hits.Count > 0;

        if (placementPoseIsValid)
        {
            ARPlane newPlacementPlane = arOrigin.GetComponent<ARPlaneManager>().GetPlane(hits[0].trackableId);

            PlaneAlignment align = newPlacementPlane.alignment;
            if (objectManager.isOnVerticalPlane)
            {
                placementPoseIsValid = align == PlaneAlignment.Vertical || align == PlaneAlignment.NotAxisAligned;
            }
            if (objectManager.isOnCeil)
            {
                placementPoseIsValid = align == PlaneAlignment.HorizontalDown;
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
