using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleDoor : SpecialIneraction
{
    public List<Transform> rightDoor; //the right door
    public List<Transform> leftDoor; //the left door

    public float endRotation = 120f; //the end rotation
    public float startRotation = 0f; //the start rotation
    public float speed = 40f; //the speed the door opens
    [SerializeField]
    private bool isOpening = false;
    private CheckValidObject check;
    [SerializeField]
    private List<SBoxCollider> rightCollider;
    [SerializeField]
    private List<SBoxCollider> leftCollider;
    IEnumerator OpenDoor() //declares a coroutine
    {
        while (true) //while the current rotation is less than the end we will continue
        {
            bool canBreak = true;
            Vector3 newEulerAngles;

            if (leftDoor != null && leftDoor.Count > 0)
            {
                foreach (Transform door in leftDoor)
                {
                    if (door.localRotation.eulerAngles.y < endRotation || door.localRotation.eulerAngles.y > 180)
                    {
                        canBreak = false;
                        newEulerAngles = door.localRotation.eulerAngles + new Vector3(0, speed * Time.deltaTime, 0);
                        door.localRotation = Quaternion.Euler(newEulerAngles);
                    }
                }
            }

            if (rightDoor != null && rightDoor.Count > 0)
            {
                // Debug.Log(rightDoor.localRotation.eulerAngles);
                foreach (Transform door in rightDoor)
                {
                    if (door.localRotation.eulerAngles.y > (360 - endRotation) || door.localRotation.eulerAngles.y < 180)
                    {
                        canBreak = false;
                        newEulerAngles = door.localRotation.eulerAngles - new Vector3(0, speed * Time.deltaTime, 0);
                        door.localRotation = Quaternion.Euler(newEulerAngles);
                    }
                }
            }


            bool isValid = true;
            foreach (SBoxCollider collider in rightCollider)
            {
                isValid &= !collider.isHit;
            }
            foreach (SBoxCollider collider in leftCollider)
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
    IEnumerator CloseDoor()
    {
        while (true) //while the current rotation is less than the end we will continue
        {
            bool canBreak = true;
            Vector3 newEulerAngles;

            if (leftDoor != null && leftDoor.Count > 0)
            {
                foreach (Transform door in leftDoor)
                {
                    if (door.localRotation.eulerAngles.y > startRotation && door.localRotation.eulerAngles.y < 180)
                    {
                        canBreak = false;
                        newEulerAngles = door.localRotation.eulerAngles - new Vector3(0, speed * Time.deltaTime, 0);
                        door.localRotation = Quaternion.Euler(newEulerAngles);
                    }
                }
            }

            if (rightDoor != null && rightDoor.Count > 0)
            {
                // Debug.Log(rightDoor.localRotation.eulerAngles);
                foreach (Transform door in rightDoor)
                {
                    if (door.localRotation.eulerAngles.y < (360 - startRotation) && door.localRotation.eulerAngles.y > 180)
                    {
                        canBreak = false;
                        newEulerAngles = door.localRotation.eulerAngles + new Vector3(0, speed * Time.deltaTime, 0);
                        door.localRotation = Quaternion.Euler(newEulerAngles);
                    }
                }
            }

            bool isValid = true;
            foreach (SBoxCollider collider in rightCollider)
            {
                isValid &= !collider.isHit;
            }
            foreach (SBoxCollider collider in leftCollider)
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
        rightCollider = new List<SBoxCollider>();
        leftCollider = new List<SBoxCollider>();
        if (rightDoor != null && rightDoor.Count > 0)
        {
            foreach (Transform door in rightDoor)
            {
                SBoxCollider collider = door.gameObject.AddComponent<SBoxCollider>();
                collider.m_LayerMask = check.m_LayerMask;
                collider.excludeGO.Add(this.gameObject);

                rightCollider.Add(collider);
            }
            foreach (SBoxCollider collider in rightCollider)
                foreach (SBoxCollider rCollider in rightCollider)
                    if (collider != rCollider)
                    {
                        collider.excludeGO.Add(rCollider.gameObject);
                    }
        }
        if (leftDoor != null && leftDoor.Count > 0)
        {
            foreach (Transform door in leftDoor)
            {
                SBoxCollider collider = door.gameObject.AddComponent<SBoxCollider>();
                collider.m_LayerMask = check.m_LayerMask;
                collider.excludeGO.Add(this.gameObject);

                leftCollider.Add(collider);
            }
            foreach (SBoxCollider collider in leftCollider)
                foreach (SBoxCollider lCollider in leftCollider)
                    if (collider != lCollider)
                    {
                        collider.excludeGO.Add(lCollider.gameObject);
                    }
        }
        foreach (SBoxCollider rCollider in rightCollider)
            foreach (SBoxCollider lCollider in leftCollider)
            {
                rCollider.excludeGO.Add(lCollider.gameObject);
                lCollider.excludeGO.Add(rCollider.gameObject);
            }


        if (isOpening)
            StartCoroutine("OpenDoor");
        else
            StartCoroutine("CloseDoor");
    }

    public override void StopAction()
    {
        StopAllCoroutines();
        check.OnDeselect();
        if (rightCollider.Count > 0)
            foreach (SBoxCollider collider in rightCollider)
                DestroyImmediate(collider);
        if (leftCollider.Count > 0)
            foreach (SBoxCollider collider in leftCollider)
                DestroyImmediate(collider);
    }

    private void Awake()
    {
        check = GameObject.Find("Interaction").GetComponent<CheckValidObject>();
        rightCollider = new List<SBoxCollider>();
        leftCollider = new List<SBoxCollider>();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
