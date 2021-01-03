using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectManager : MonoBehaviour
{
    public GameObject targetGameObject;
    public GameObject parentPrefab;
    [SerializeField]
    public GameObject deployParent;
    private RoomManager roomManager;
    private ResizeObject resizeComponent;
    private AddRemoveObject addRemoveObject;
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

        roomManager = transform.GetComponent<RoomManager>();
        resizeComponent = GameObject.Find("Interaction").GetComponent<ResizeObject>();
        addRemoveObject = transform.GetComponent<AddRemoveObject>();
        boxComponent = GameObject.Find("Interaction").GetComponent<BoxTransform>();

        var test = ABUtils.Instance;
    }

    public void setTargetGameObject(GameObject gameObject)
    {
        targetGameObject = gameObject;
        bool enable = (gameObject != null);
        boxComponent.enabled = enable;
        resizeComponent.setBtn(enable);
        addRemoveObject.setAnRBtns(!enable);
        roomManager.setBtn(!enable);
        roomManager.isEdit = true;
    }

    public void deployObject(GameObject gameObject, DeployObjectProperty property)
    {
        GameObject parent = Instantiate(parentPrefab, Vector3.zero, Quaternion.identity);
        GameObject children = Instantiate(gameObject, Vector3.zero, Quaternion.identity);
        setBoxCollider(children);
        children.transform.SetParent(parent.transform);

        BoxCollider boxCollider = children.GetComponent<BoxCollider>();
        Vector3 pos = -boxCollider.center + new Vector3(0, boxCollider.size.y / 2, 0);
        if (property.isOnCeil)
            pos.y -= boxCollider.size.y;
        if (property.isOnVerticalPlane)
        {
            pos.y -= boxCollider.size.y / 2;
            pos.z -= boxCollider.size.z / 2;
        }
        children.transform.localPosition = pos;

        foreach (Transform trans in parent.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = LayerMask.NameToLayer(deployLayer);
        }

        Vector3 position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 5));
        parent.transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, 0));
        parent.name = gameObject.name;

        parent.transform.parent = deployParent.transform;

        roomManager.isEdit = true;
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
}
