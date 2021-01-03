using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

public class ToggleAR : MonoBehaviour
{
    private ARSessionOrigin arOrigin;
    private ARPlaneManager arPlaneManager;
    private ARPointCloudManager arPointCloudManager;
    private bool isEnable;
    ToggleButton button;
    // Start is called before the first frame update
    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arPointCloudManager = arOrigin.GetComponent<ARPointCloudManager>();
        arPlaneManager = arOrigin.GetComponent<ARPlaneManager>();

        arPlaneManager.enabled = true;
        arPointCloudManager.enabled = true;
        isEnable = true;

        button = this.transform.GetComponent<ToggleButton>();
        button.setHold(isEnable);
    }

    public void toggleState()
    {
        isEnable = !isEnable;

        if (arPlaneManager != null)
        {
            arPlaneManager.enabled = isEnable;
            foreach (ARPlane plane in arPlaneManager.trackables)
            {
                plane.gameObject.SetActive(arPlaneManager.enabled);
            }
        }

        if (arPointCloudManager != null)
        {
            arPointCloudManager.enabled = isEnable;
            GameObject goPointCloud = arPointCloudManager.pointCloudPrefab;
            if (goPointCloud != null)
            {
                var point = GameObject.Find(goPointCloud.name + "(Clone)");
                if (point != null)
                {
                    point.SetActive(arPointCloudManager.enabled);
                }
            }
        }

        button.setHold(isEnable);
    }
}
