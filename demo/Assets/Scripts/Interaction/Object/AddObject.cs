using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddObject : MonoBehaviour
{
    [SerializeField]
    private Transform addUI;
    private GameObject panel;
    private GameObject btn;
    private bool isHold;

    // Start is called before the first frame update
    void Start()
    {
        panel = addUI.GetChild(0).gameObject;
        btn = addUI.GetChild(1).gameObject;
    }

    public void onBtnClick()
    {
        isHold = !isHold;
        if (isHold)
            OnSelected();
        else
            OnDeselected();
    }

    public void OnSelected()
    {
        panel.GetComponent<Animator>().ResetTrigger("MenuOut");
        panel.GetComponent<Animator>().SetTrigger("MenuIn");
        btn.GetComponent<ToggleButton>().setHold(true);
    }

    public void OnDeselected()
    {
        panel.GetComponent<Animator>().ResetTrigger("MenuIn");
        panel.GetComponent<Animator>().SetTrigger("MenuOut");
        btn.GetComponent<ToggleButton>().setHold(false);
    }
}
