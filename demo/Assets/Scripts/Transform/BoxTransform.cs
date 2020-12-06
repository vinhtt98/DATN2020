using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTransform : MonoBehaviour
{
    // Start is called before the first frame update
    public float width = 0.02f;
    [SerializeField]
    private GameObject _vertexPrefab;
    [SerializeField]
    private GameObject _edgePrefab;
    [SerializeField]
    private Transform parentObject;
    private GameObject targetGameObject;
    private LineRenderer[] lineRenderers;
    private Vector3 v3FrontTopLeft;
    private Vector3 v3FrontTopRight;
    private Vector3 v3FrontBottomLeft;
    private Vector3 v3FrontBottomRight;
    private Vector3 v3BackTopLeft;
    private Vector3 v3BackTopRight;
    private Vector3 v3BackBottomLeft;
    private Vector3 v3BackBottomRight;
    private GameObject[] edges;
    private GameObject[] vertices;

    private void Start() {
        lineRenderers = new LineRenderer[12];
        for (int i=0; i<12; i++) {
            lineRenderers[i] = new GameObject().AddComponent<LineRenderer>();
            lineRenderers[i].startWidth = width;
            lineRenderers[i].endWidth = width;
            lineRenderers[i].material = new Material(Shader.Find("Unlit/Texture"));
            lineRenderers[i].transform.SetParent(parentObject, false);
        }

        edges = new GameObject[12];
        for (int i=0; i<12; i++) {
            edges[i] = Instantiate(_edgePrefab, new Vector3(0, 0, 0), Quaternion.identity, parentObject);
        }

        vertices = new GameObject[8];
        for (int i=0; i<8; i++) {
            vertices[i] = Instantiate(_vertexPrefab, new Vector3(0, 0, 0), Quaternion.identity, parentObject);
        }
    }

    void OnEnable() {
        targetGameObject = GameObject.Find("GameManager").GetComponent<ObjectManager>().targetGameObject.transform.GetChild(0).gameObject;
        parentObject.gameObject.SetActive(true);
    }

    void OnDisable() {
        parentObject.gameObject.SetActive(false);
    }
     
    void Update() {
        CalcPositons();
        DrawBox();
        SetEdgesPosition();
        SetVerticesPosition();
    }   
        
    void CalcPositons(){
        BoxCollider b = targetGameObject.GetComponent<BoxCollider>();
 
        v3FrontTopLeft     = targetGameObject.transform.TransformPoint(b.center + new Vector3(-b.size.x, b.size.y, -b.size.z)*0.5f);
        v3FrontTopRight    = targetGameObject.transform.TransformPoint(b.center + new Vector3(b.size.x, b.size.y, -b.size.z)*0.5f);
        v3FrontBottomLeft  = targetGameObject.transform.TransformPoint(b.center + new Vector3(-b.size.x, -b.size.y, -b.size.z)*0.5f);
        v3FrontBottomRight = targetGameObject.transform.TransformPoint(b.center + new Vector3(b.size.x, -b.size.y, -b.size.z)*0.5f);
        v3BackTopLeft      = targetGameObject.transform.TransformPoint(b.center + new Vector3(-b.size.x, b.size.y, b.size.z)*0.5f);
        v3BackTopRight     = targetGameObject.transform.TransformPoint(b.center + new Vector3(b.size.x, b.size.y, b.size.z)*0.5f);
        v3BackBottomLeft   = targetGameObject.transform.TransformPoint(b.center + new Vector3(-b.size.x, -b.size.y, b.size.z)*0.5f);
        v3BackBottomRight  = targetGameObject.transform.TransformPoint(b.center + new Vector3(b.size.x, -b.size.y, b.size.z)*0.5f);
    }

    void DrawLine(Vector3 start, Vector3 end, LineRenderer lr) {
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
     
    void DrawBox() {
        //if (Input.GetKey (KeyCode.S)) {
        DrawLine (v3FrontTopLeft, v3FrontTopRight, lineRenderers[0]);
        DrawLine (v3FrontTopRight, v3FrontBottomRight, lineRenderers[1]);
        DrawLine (v3FrontBottomRight, v3FrontBottomLeft, lineRenderers[2]);
        DrawLine (v3FrontBottomLeft, v3FrontTopLeft, lineRenderers[3]);
            
        DrawLine (v3BackTopLeft, v3BackTopRight, lineRenderers[4]);
        DrawLine (v3BackTopRight, v3BackBottomRight, lineRenderers[5]);
        DrawLine (v3BackBottomRight, v3BackBottomLeft, lineRenderers[6]);
        DrawLine (v3BackBottomLeft, v3BackTopLeft, lineRenderers[7]);
            
        DrawLine (v3FrontTopLeft, v3BackTopLeft, lineRenderers[8]);
        DrawLine (v3FrontTopRight, v3BackTopRight, lineRenderers[9]);
        DrawLine (v3FrontBottomRight, v3BackBottomRight, lineRenderers[10]);
        DrawLine (v3FrontBottomLeft, v3BackBottomLeft, lineRenderers[11]);
        //}
    }

    void SetMiddlePosition(Vector3 v1, Vector3 v2, GameObject go) {
        go.transform.SetPositionAndRotation((v1+v2)*0.5f, transform.rotation);
    }

    void SetEdgesPosition() {
        SetMiddlePosition (v3FrontTopLeft, v3FrontTopRight, edges[0]);
        SetMiddlePosition (v3FrontTopRight, v3FrontBottomRight, edges[1]);
        SetMiddlePosition (v3FrontBottomRight, v3FrontBottomLeft, edges[2]);
        SetMiddlePosition (v3FrontBottomLeft, v3FrontTopLeft, edges[3]);
            
        SetMiddlePosition (v3BackTopLeft, v3BackTopRight, edges[4]);
        SetMiddlePosition (v3BackTopRight, v3BackBottomRight, edges[5]);
        SetMiddlePosition (v3BackBottomRight, v3BackBottomLeft, edges[6]);
        SetMiddlePosition (v3BackBottomLeft, v3BackTopLeft, edges[7]);
            
        SetMiddlePosition (v3FrontTopLeft, v3BackTopLeft, edges[8]);
        SetMiddlePosition (v3FrontTopRight, v3BackTopRight, edges[9]);
        SetMiddlePosition (v3FrontBottomRight, v3BackBottomRight, edges[10]);
        SetMiddlePosition (v3FrontBottomLeft, v3BackBottomLeft, edges[11]);
    }

    void SetVerticesPosition() {
        vertices[0].transform.SetPositionAndRotation(v3FrontTopLeft, transform.rotation);
        vertices[1].transform.SetPositionAndRotation(v3FrontTopRight, transform.rotation);
        vertices[2].transform.SetPositionAndRotation(v3FrontBottomLeft, transform.rotation);
        vertices[3].transform.SetPositionAndRotation(v3FrontBottomRight, transform.rotation);
        vertices[4].transform.SetPositionAndRotation(v3BackTopLeft, transform.rotation);
        vertices[5].transform.SetPositionAndRotation(v3BackTopRight, transform.rotation);
        vertices[6].transform.SetPositionAndRotation(v3BackBottomLeft, transform.rotation);
        vertices[7].transform.SetPositionAndRotation(v3BackBottomRight, transform.rotation);
    }
}
