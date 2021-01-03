using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRemoveObject : MonoBehaviour
{
    [SerializeField]
    private Transform addUI;
    private GameObject panel;
    private GameObject addBtn;
    [SerializeField]
    private GameObject removeBtn;
    private bool isHold;
    private ObjectManager objectManager;
    private void Awake()
    {
        objectManager = GameObject.FindObjectOfType<ObjectManager>();
        panel = addUI.GetChild(0).gameObject;
        addBtn = addUI.GetChild(1).gameObject;
    }

    public void setAnRBtns(bool value) {
        addBtn.SetActive(value);
        removeBtn.SetActive(!value);
    }

    public void onAddBtnClick()
    {
        isHold = !isHold;
        if (isHold)
            OnSelected();
        else
            OnDeselected();
    }

    public void onRemoveBtnClick()
    {
        Destroy(objectManager.targetGameObject);
        objectManager.setTargetGameObject(null);
    }

    public void OnSelected()
    {
        panel.GetComponent<Animator>().ResetTrigger("MenuOut");
        panel.GetComponent<Animator>().SetTrigger("MenuIn");
        addBtn.GetComponent<ToggleButton>().setHold(true);
    }

    public void OnDeselected()
    {
        panel.GetComponent<Animator>().ResetTrigger("MenuIn");
        panel.GetComponent<Animator>().SetTrigger("MenuOut");
        addBtn.GetComponent<ToggleButton>().setHold(false);
    }
}
