using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Globalization;
using TMPro;

public class ResizeObject : MonoBehaviour
{
    [SerializeField]
    private Transform resizeUI;
    private TMP_InputField lengthField;
    private TMP_InputField widthField;
    private TMP_InputField heightField;
    private GameObject resizePanel;
    private GameObject resizeBtn;
    private GameObject targetGameObject;
    private Vector3 bSize;
    private Vector3 scale;
    private Vector3 pivotPoint;
    private bool isHold;

    void Start()
    {
        resizePanel = resizeUI.GetChild(0).gameObject;
        resizeBtn = resizeUI.GetChild(1).gameObject;

        lengthField = resizePanel.transform.GetChild(1).GetComponent<TMP_InputField>();
        widthField = resizePanel.transform.GetChild(3).GetComponent<TMP_InputField>();
        heightField = resizePanel.transform.GetChild(5).GetComponent<TMP_InputField>();

        bSize = Vector3.zero;
        scale = Vector3.zero;
    }

    public void onBtnClick()
    {
        isHold = !isHold;
        if (isHold)
            OnSelected();
        else
            OnDeselected();
    }

    public void setBtn(bool enable)
    {
        resizeBtn.SetActive(enable);
        isHold = false;
    }

    public void OnSelected()
    {
        ObjectManager objectManager = GameObject.Find("GameManager").GetComponent<ObjectManager>();
        targetGameObject = objectManager.targetGameObject;

        targetGameObject.GetComponent<InteractionObject>().SetSpecialInteraction(false);

        resizePanel.GetComponent<Animator>().ResetTrigger("MakeTransitionOut");
        resizePanel.GetComponent<Animator>().SetTrigger("MakeTransitionIn");
        resizeBtn.GetComponent<ToggleButton>().setHold(true);

        bSize = objectManager.objectSize;
        scale = targetGameObject.transform.localScale;

        pivotPoint = targetGameObject.transform.position;

        Display();
    }

    public void OnDeselected()
    {
        Animator objectAnimator = resizePanel.GetComponent<Animator>();
        resizePanel.GetComponent<Animator>().ResetTrigger("MakeTransitionIn");
        resizePanel.GetComponent<Animator>().SetTrigger("MakeTransitionOut");
        resizeBtn.GetComponent<ToggleButton>().setHold(false);

        bSize = Vector3.zero;
        scale = Vector3.zero;
    }

    void Display()
    {
        lengthField.text = floatToString(bSize.x * scale.x);
        widthField.text = floatToString(bSize.z * scale.z);
        heightField.text = floatToString(bSize.y * scale.y);
    }

    String floatToString(float value)
    {
        return String.Format("{0:0.##}", value);
    }

    float stringToFloat(String str, float value)
    {
        str.Replace(',', '.');
        try
        {
            float number = float.Parse(str, CultureInfo.InvariantCulture);
            return number;
        }
        catch
        {
            return value;
        }
    }

    private Vector3 calc(Vector3 newPoint, Vector3 oldPoint)
    {
        Vector3 lineVec = newPoint - pivotPoint;
        Vector3 oldLineVec = oldPoint - pivotPoint;
        Quaternion quat = targetGameObject.transform.rotation;
        //Inverse
        quat = Quaternion.Inverse(quat);
        //Multiply with quarternion for right direction
        //Quaternion * Vector3 || Quaternion.Inverse(Quaternion) * Vector3
        lineVec = quat * lineVec;
        oldLineVec = quat * oldLineVec;

        // Debug.Log("lineVec "+lineVec);
        // Debug.Log("oldLineVec "+oldLineVec);
        // Debug.Log("Quat "+(Quaternion) targetGameObject.transform.rotation);
        // Debug.Log("Vec3 Quat "+targetGameObject.transform.rotation);

        // Chan truong hop:
        // oldLine = 0
        // resize = am (lineVec * oldLineVec = 0)
        if (lineVec.x * oldLineVec.x <= 0)
            return Vector3.one;
        if (lineVec.y * oldLineVec.y <= 0)
            return Vector3.one;
        if (lineVec.z * oldLineVec.z <= 0)
            return Vector3.one;

        return new Vector3(lineVec.x / oldLineVec.x, lineVec.y / oldLineVec.y, lineVec.z / oldLineVec.z);
    }

    public void OnDrag(Vector3 newPoint, Vector3 oldPoint)
    {
        Vector3 res = calc(newPoint, oldPoint);

        scale = targetGameObject.transform.localScale;

        scale.x *= Mathf.Abs(res.x);
        scale.z *= Mathf.Abs(res.z);
        scale.y *= Mathf.Abs(res.y);

        targetGameObject.transform.localScale = scale;

        Display();
    }

    public void OnApply()
    {
        scale = targetGameObject.transform.localScale;

        float newLength = stringToFloat(lengthField.text, bSize.x * scale.x);
        float newWidth = stringToFloat(widthField.text, bSize.z * scale.z);
        float newHeight = stringToFloat(heightField.text, bSize.y * scale.y);

        scale.x = newLength / bSize.x;
        scale.z = newWidth / bSize.z;
        scale.y = newHeight / bSize.y;

        targetGameObject.transform.localScale = scale;
    }
}
