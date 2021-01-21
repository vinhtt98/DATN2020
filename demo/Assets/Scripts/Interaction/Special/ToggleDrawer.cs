using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleDrawer : SpecialIneraction
{
    public List<Transform> drawers; //list of drawers
    public float drawLength = 0.5f;
    [SerializeField]
    private float startPoint = float.MaxValue; //the start rotation
    public float speed = 0.15f; //the speed the door opens
    [SerializeField]
    private bool isOpening = false;
    private CheckValidObject check;
    [SerializeField]
    private List<SBoxCollider> sCollider;
    IEnumerator OpenDrawer() //declares a coroutine
    {
        while (true) //while the current rotation is less than the end we will continue
        {
            bool canBreak = true;

            if (drawers != null && drawers.Count > 0)
            {
                foreach (Transform drawer in drawers)
                {
                    if (drawer.localPosition.z < startPoint + drawLength)
                    {
                        canBreak = false;
                        drawer.localPosition += new Vector3(0, 0, speed * Time.deltaTime);
                    }
                }
            }

            bool isValid = true;
            foreach (SBoxCollider collider in sCollider)
            {
                isValid &= !collider.isHit;
            }
            check.UpdateValid(isValid);

            if (canBreak)
                break;

            yield return null; //returns null and will start up here again next frame, yet it will just restart the while loop
        }
        // Debug.Log("Open end");
        StopAction();
    }
    IEnumerator CloseDrawer()
    {
        while (true) //while the current rotation is less than the end we will continue
        {
            bool canBreak = true;

            if (drawers != null && drawers.Count > 0)
            {
                foreach (Transform drawer in drawers)
                {
                    if (drawer.localPosition.z > startPoint)
                    {
                        canBreak = false;
                        drawer.localPosition -= new Vector3(0, 0, speed * Time.deltaTime);
                    }
                }
            }

            bool isValid = true;
            foreach (SBoxCollider collider in sCollider)
            {
                isValid &= !collider.isHit;
            }
            check.UpdateValid(isValid);

            if (canBreak)
                break;

            yield return null; //returns null and will start up here again next frame, yet it will just restart the while loop
        }
        // Debug.Log("Close end");
        StopAction();
    }

    public override void DoAction()
    {
        isOpening = !isOpening;

        StopAction();

        check.OnSelected();
        sCollider = new List<SBoxCollider>();
        if (drawers != null && drawers.Count > 0)
        {
            foreach (Transform drawer in drawers)
            {
                SBoxCollider collider = drawer.gameObject.AddComponent<SBoxCollider>();
                collider.m_LayerMask = check.m_LayerMask;
                collider.excludeGO.Add(this.gameObject);

                sCollider.Add(collider);

                startPoint = Mathf.Min(startPoint, drawer.localPosition.z);
            }
            foreach (SBoxCollider collider in sCollider)
                foreach (SBoxCollider sCollider in sCollider)
                    if (collider != sCollider)
                    {
                        collider.excludeGO.Add(sCollider.gameObject);
                    }
        }

        if (isOpening)
            StartCoroutine("OpenDrawer");
        else
            StartCoroutine("CloseDrawer");
    }

    public override void StopAction()
    {
        StopAllCoroutines();
        check.OnDeselect();
        if (sCollider.Count > 0)
            foreach (SBoxCollider collider in sCollider)
                DestroyImmediate(collider);
    }

    private void Awake()
    {
        check = GameObject.Find("Interaction").GetComponent<CheckValidObject>();
        sCollider = new List<SBoxCollider>();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
