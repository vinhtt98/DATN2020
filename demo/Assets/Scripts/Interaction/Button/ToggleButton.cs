using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    public Sprite btnSprite;
    public Sprite holdBtnSprite;
    private bool isHold;
    // Start is called before the first frame update
    void Start()
    {
        isHold = false;
        transform.GetComponent<Image>().sprite = btnSprite;
    }

    public void onClick() {
        isHold = !isHold;
        if (isHold)
            transform.GetComponent<Image>().sprite = holdBtnSprite;
        else
            transform.GetComponent<Image>().sprite = btnSprite;
        Debug.Log("OnClick");
    }

    public void setHold(bool value) {
        isHold = value;
        if (isHold)
            transform.GetComponent<Image>().sprite = holdBtnSprite;
        else
            transform.GetComponent<Image>().sprite = btnSprite;
        Debug.Log("OnSetHold");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
