using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    Slider slider;
    private void Start()
    {
        ABUtils.Instance.LoadAllAsset(slider);
    }

    public void OnSliderValueChange()
    {
        if (slider.value != slider.maxValue)
            return;

        OnLoadComplete();
        GameObject.FindObjectOfType<AddPanel>().makeAddPanel();

        Destroy(this.gameObject);
    }

    private void OnLoadComplete()
    {
        AddPanel addPanel = GameObject.FindObjectOfType<AddPanel>();

        foreach (int id in ABUtils.goDict.Keys)
        {
            addPanel.addPrefab(ABUtils.tagDict[id], ABUtils.goDict[id]);
        }
    }
}
