using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckValidObject : MonoBehaviour
{
    public LayerMask m_LayerMask;
    private ObjectManager objectManager;
    [SerializeField]
    private Material red;
    [SerializeField]
    private Material green;
    private ArrayList oldColors;
    private GameObject targetGameObject;
    private Renderer[] targetRenderers;
    //Start function
    void Start()
    {
        objectManager = GameObject.Find("GameManager").GetComponent<ObjectManager>();
    }
    public void OnSelected()
    {
        targetGameObject = GameObject.Find("GameManager").GetComponent<ObjectManager>().targetGameObject.transform.GetChild(0).gameObject;
        targetRenderers = targetGameObject.GetComponentsInChildren<MeshRenderer>();
        if (targetRenderers != null)
        {
            oldColors = new ArrayList(targetRenderers.Length);
            foreach (var renderer in targetRenderers)
            {
                ArrayList oldColor = new ArrayList(renderer.materials.Length);
                foreach (var material in renderer.materials)
                {
                    oldColor.Add(material.color);
                }
                oldColors.Add(oldColor);
            }
        }
    }

    private bool checkValid()
    {
        Transform transform = targetGameObject.transform;
        BoxCollider b = transform.GetComponent<BoxCollider>();

        Vector3 bSize = b.size;
        Vector3 scale = transform.parent.localScale;

        Vector3 pos = transform.TransformPoint(b.center);
        Vector3 halfExtend = new Vector3(bSize.x * scale.x, bSize.y * scale.y, bSize.z * scale.z) / 2;

        Collider[] hitColliders = Physics.OverlapBox(pos, halfExtend, transform.rotation, m_LayerMask);
        //Check when there is a new collider coming into contact with the box
        foreach (Collider hitCollider in hitColliders) {
            //Output all of the collider names
            Debug.Log("Hit : " + hitCollider.name);
            if (hitCollider.transform != transform)
                return false;
        }
        return true;
    }

    public void UpdateValid()
    {
        bool isValid = checkValid();
        if (isValid)
            fillObjecct(green);
        else
            fillObjecct(red);
        objectManager.isValidPosition = isValid;
    }

    private void fillObjecct(Material val)
    {
        if (targetRenderers != null)
        {
            foreach (var renderer in targetRenderers)
            {
                foreach (var material in renderer.materials)
                {
                    material.color = val.color;
                }
            }
        }
    }

    public void OnDeselect()
    {
        if (targetRenderers != null)
        {
            int i = 0;
            foreach (var renderer in targetRenderers)
            {
                ArrayList oldColor = (ArrayList)oldColors[i];
                int j = 0;
                foreach (var material in renderer.materials)
                {
                    material.color = (Color)oldColor[j];
                    j++;
                }
                i++;
            }
        }
    }
}
