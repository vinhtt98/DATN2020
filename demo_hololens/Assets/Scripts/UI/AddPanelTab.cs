using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class AddPanelTab : ScrollPanel
{
    protected override void Awake()
    {
        base.Awake();
    }
    public GameObject addTab(string name)
    {
        GameObject tab = Instantiate(cellPrefab, Vector3.zero, Quaternion.identity, this.transform);

        tab.name = name;
        tab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        tab.GetComponent<Button>().onClick.AddListener(delegate { OnTabClick(tab); });

        dict.Add(name, tab);

        return tab;
    }

    public void OnTabClick(GameObject tab) {
        GameObject.FindObjectOfType<AddPanel>().switchTab(tab);
    }
}
