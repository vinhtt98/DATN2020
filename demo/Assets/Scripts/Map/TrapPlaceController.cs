using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class TrapPlaceController : MonoBehaviour
{
    private GameObject currentPlaceableObject;

    [SerializeField]
    private LayerMask _attackableLayer;

    // [SerializeField]
    private float hitRadius = 0.1f;

    [SerializeField]
    private float mouseWheelRotation;
    private int currentPrefabIndex = -1;

    private bool isPlaceable = false;

    [SerializeField]
    private Image cooldownImage;

    private float coolDownTime;

    private bool isCoolingDown = false;
    Vector3 truePos;

    Collider _trapCollider;

    Collider[] roadColliders;

    void Start() {
        cooldownImage.gameObject.SetActive(false);
    }

    private void Update() {
        if (cooldownImage.fillAmount > 0) {
            isCoolingDown = true;
            cooldownImage.fillAmount -= 1 / coolDownTime * Time.deltaTime;
        } else {
            isCoolingDown = false;
        }
    }

    private void LateUpdate()
    {
        HandleNewObjectHotkey();

        if (currentPlaceableObject != null)
        {
            MoveCurrentObjectToMouse();
            ReleaseIfClicked();
        }
    }

    private void HandleNewObjectHotkey()
    {

        if (Input.GetKeyDown(KeyCode.B)) {
            if (currentPlaceableObject != null) {
                Destroy(currentPlaceableObject);
            }

            // currentPlaceableObject = TrapFactory.SpawnDummyTrap(TrapType.Booby, new Vector3(0,0,0));
            // currentPlaceableObject.transform.rotation = GameObject.FindGameObjectWithTag("Stage").transform.rotation;
        }
    }

    public void onButtonClick() {
        if (isCoolingDown) {
            return;
        }
        if (currentPlaceableObject != null) {
                Destroy(currentPlaceableObject);
            }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;

        Vector3 position = Vector3.zero;

        if (Physics.Raycast(ray, out hitInfo))
        {
            position = hitInfo.point;
        }

        // currentPlaceableObject = TrapFactory.SpawnDummyTrap(_trapType, position);
        _trapCollider = currentPlaceableObject.GetComponent<Collider>();
        // currentPlaceableObject.transform.rotation = GameObject.FindGameObjectWithTag("Stage").transform.rotation;

    }

    private bool PressedKeyOfCurrentPrefab(int i)
    {
        return currentPlaceableObject != null && currentPrefabIndex == i;
    }

    private void MoveCurrentObjectToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            // currentPlaceableObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);

            Collider[] _colliders = new Collider[1];
            
            // float gridSize = GameManager.Instance().gridSize;
            float gridSize = 0;
            truePos.x = Mathf.Floor(hitInfo.point.x / gridSize) * gridSize;
            truePos.y = GameObject.Find("Road").transform.position.y;
            truePos.z = Mathf.Floor(hitInfo.point.z / gridSize) * gridSize;

            Vector3 centerPoint = truePos;
            centerPoint.x = truePos.x + 0.5f;
            centerPoint.z = truePos.z + 0.5f;

            Debug.DrawLine (Camera.main.transform.position, centerPoint, Color.red);

            currentPlaceableObject.transform.position = truePos;

            int minXInside = 0, minYInside = 0;
            int maxXInside = 0, maxYInside = 0;
            // Physics.OverlapSphereNonAlloc(centerPoint, hitRadius, _colliders, _attackableLayer);
            // foreach (Collider collider in GameManager.Instance().getRoadColliders()) {
            //     if (collider != null) {
            //         Bounds trapBounds = _trapCollider.bounds;
            //         if (isPointInside(collider.bounds, new Vector2(trapBounds.min.x, trapBounds.min.z))) {
            //             minXInside++;
            //         }
            //         if (isPointInside(collider.bounds, new Vector2(trapBounds.min.x + trapBounds.size.x, trapBounds.min.z))) {
            //             minYInside++;
            //         }
            //         if (isPointInside(collider.bounds, new Vector2(trapBounds.min.x, trapBounds.min.z + trapBounds.size.z))) {
            //             maxXInside++;
            //         }
            //         if (isPointInside(collider.bounds, new Vector2(trapBounds.max.x, trapBounds.max.z))) {
            //             maxYInside++;
            //         }
            //     }
            // }

            if (minXInside == minYInside && minYInside == maxXInside && maxXInside == maxYInside && minYInside > 0)
            {
                isPlaceable = true;
                Renderer renderer = currentPlaceableObject.GetComponent<Renderer>();
                
                if (renderer != null) {
                    Material[] mat = renderer.materials.Select(x => {
                        Material i = x;
                        i.color = Color.green;
                        return i;
                    }).ToArray();
                    renderer.materials = mat;
                }
            } 
            else
            {
                isPlaceable = false;
                Renderer renderer = currentPlaceableObject.GetComponent<Renderer>();
                
                if (renderer != null) {
                    Material[] mat = renderer.materials.Select(x => {
                        Material i = x;
                        i.color = Color.red;
                        return i;
                    }).ToArray();
                    renderer.materials = mat;
                }
            }
            

        } else {
            Debug.Log("Failed");
        }
    }

    void StartCoolDown() {
        isCoolingDown = true;
        cooldownImage.gameObject.SetActive(true);
        cooldownImage.fillAmount = 1;
    }

    private void ReleaseIfClicked()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (isPlaceable) {
                // GameObject trap = TrapFactory.SpawnTrap(_trapType, currentPlaceableObject.transform.position);
                // DestroyImmediate(currentPlaceableObject);
                // TrapController controller = trap.GetComponent<TrapController>();
                // if (controller != null) {
                //     Debug.Log("Placed!!!");
                //     coolDownTime = controller.GetProperties().Cooldown;
                //     controller.OnPlaced();
                //     StartCoolDown();
                // }

            } else {
                DestroyImmediate(currentPlaceableObject);
            }

            isPlaceable = false;
            currentPlaceableObject = null;
        }
    }

    private bool isInside(Bounds A, Bounds B) {
        return A.min.x >= B.min.x && A.min.z >= B.min.z && A.max.x <= B.max.x && A.max.z <= B.max.z;
    }

    private bool isPointInside(Bounds A, Vector2 B) {
        return A.min.x <= B.x && A.min.z <= B.y && A.max.x >= B.x && A.max.z >= B.y;
    }
}
