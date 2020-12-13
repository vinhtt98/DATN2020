using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class ObjectManager : MonoBehaviour
{
    public GameObject targetGameObject;
    private ResizeObject resizeComponent;
    private BoxTransform boxComponent;
    public Vector3 objectSize;
    public bool isCanResize;
    public bool isOnVerticalPlane;
    public bool isOnCeil;
    public bool isValidPosition;
    public string deployLayer = "DeployObject";

    // Automatically add PhysicsRaycaster to the main Camera
    void addPhysicsRaycaster()
    {
        PhysicsRaycaster physicsRaycaster = GameObject.FindObjectOfType<PhysicsRaycaster>();
        if (physicsRaycaster == null)
        {
            Camera.main.gameObject.AddComponent<PhysicsRaycaster>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        addPhysicsRaycaster();

        resizeComponent = GameObject.Find("Interaction").GetComponent<ResizeObject>();
        boxComponent = GameObject.Find("Interaction").GetComponent<BoxTransform>();

        Debug.Log(Application.persistentDataPath);

        deployObject(targetGameObject, new Vector3(0, 0, 4.78f));
    }

    public void setTargetGameObject(GameObject gameObject)
    {
        targetGameObject = gameObject;
        bool enable = (gameObject != null);
        boxComponent.enabled = enable;
        resizeComponent.setBtn(enable);
    }

    public void setObjectSize(float width, float length, float height)
    {
        objectSize = new Vector3(length, height, width);
    }

    public void deployObject(GameObject gameObject, Vector3 position)
    {
        GameObject parent = new GameObject();
        GameObject children = Instantiate(gameObject);
        setBoxCollider(children);
        children.transform.SetParent(parent.transform);

        BoxCollider boxCollider = children.GetComponent<BoxCollider>();
        Vector3 pos = -boxCollider.center + new Vector3(0, boxCollider.size.y/2, 0);
        if (isOnCeil)
            pos.y -= boxCollider.size.y;
        if (isOnVerticalPlane) {
            pos.y -= boxCollider.size.y / 2;
            pos.z -= boxCollider.size.z / 2;
        }
        children.transform.localPosition = pos;

        parent.AddComponent<InteractionObject>();
        parent.AddComponent<DragMove>();
        foreach (Transform trans in parent.GetComponentsInChildren<Transform>(true)) {
            trans.gameObject.layer = LayerMask.NameToLayer(deployLayer);
        }

        parent.transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, 0));
    }

    private void setBoxCollider(GameObject gameObject)
    {
        //Init box collider
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        if (boxCollider == null)
            boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        //Set GameObject at 0 0 0
        gameObject.transform.position = Vector3.zero;
        //Set bound at 0 0 0
        Bounds totalBound = new Bounds(Vector3.zero, Vector3.zero);
        foreach (MeshRenderer meshFilter in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            Bounds meshBounds = meshFilter.bounds;
            // Debug.Log(meshBounds);
            // meshBounds.size = Vector3.Scale(meshBounds.size, meshFilter.transform.localScale);
            // BoxCollider b = meshFilter.gameObject.GetComponent<BoxCollider>();
            // if (b != null)
            //     Debug.Log(b.bounds);
            //meshBounds.center += meshFilter.transform.localPosition;
            totalBound.Encapsulate(meshBounds);
        }
        boxCollider.size = totalBound.size;
        boxCollider.center = totalBound.center;
    }


    IEnumerator DownloadAsset()
    {
        string assetBundleName = "furniture";
        string url = "file:///" + Application.persistentDataPath + "/AssetBundles/" + assetBundleName;
        using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url, 0))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                // Get downloaded asset bundle
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
            }
        }
    }
}
