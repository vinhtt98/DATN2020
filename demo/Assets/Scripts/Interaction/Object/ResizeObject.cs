using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Globalization;
using TMPro;

public class ResizeObject : MonoBehaviour
{
    [SerializeField]
    private Transform resizeCanvas;
    public float lenght;
    private TMP_InputField lengthField;
    public float width;
    private TMP_InputField widthField;
    public float height;
    private TMP_InputField heightField;
    private GameObject resizePanel;
    private GameObject resizeBtn;
    private GameObject targetGameObject;
    private Vector3 bSize;
    private Vector3 scale;

    // Start is called before the first frame update
    void Start()
    {
        resizePanel = resizeCanvas.GetChild(0).gameObject;
        resizeBtn = resizeCanvas.GetChild(1).gameObject;

        lengthField = resizePanel.transform.GetChild(1).GetComponent<TMP_InputField>();
        widthField = resizePanel.transform.GetChild(3).GetComponent<TMP_InputField>();
        heightField = resizePanel.transform.GetChild(5).GetComponent<TMP_InputField>();
    }

    void OnEnable() {
        targetGameObject = GameObject.Find("GameManager").GetComponent<ObjectManager>().targetGameObject;
        resizePanel.GetComponent<Animator>().ResetTrigger("MakeTransitionOut");
        resizePanel.GetComponent<Animator>().SetTrigger("MakeTransitionIn");
        resizeBtn.GetComponent<ToggleButton>().setHold(true);

        bSize = targetGameObject.transform.GetChild(0).GetComponent<BoxCollider>().size;
        scale = targetGameObject.transform.localScale;

        lengthField.text = floatToString(bSize.x * scale.x);
        widthField.text = floatToString(bSize.z * scale.z);
        heightField.text = floatToString(bSize.y * scale.y);
    }

    String floatToString(float value) {
        return String.Format("{0:0.##}", value);
    }

    float stringToFloat(String str, float value) {
        str.Replace(',','.');
        try {
            float number = float.Parse(str, CultureInfo.InvariantCulture);
            return number;
        }
        catch {
            return value;
        }
    }

    void OnDisable() {
        resizePanel.GetComponent<Animator>().ResetTrigger("MakeTransitionIn");
        resizePanel.GetComponent<Animator>().SetTrigger("MakeTransitionOut");
        resizeBtn.GetComponent<ToggleButton>().setHold(false);
    }

    // Update is called once per frame
    void Update()
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
