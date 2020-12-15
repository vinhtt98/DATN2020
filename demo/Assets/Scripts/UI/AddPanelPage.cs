using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddPanelPage : ScrollPanel
{
    protected override void Awake()
    {
        base.Awake();
    }
    public void addThumbnail(GameObject prefab)
    {
        GameObject tnPrefab = Instantiate(cellPrefab, Vector3.zero, Quaternion.identity, this.transform);
        AddObject addObject = GameObject.FindObjectOfType<AddObject>();

        Rect rect = tnPrefab.GetComponent<RectTransform>().rect;
        tnPrefab.GetComponent<RawImage>().texture = RuntimePreviewGenerator.GenerateModelPreview(prefab.transform, (int)rect.width, (int)rect.height);

        tnPrefab.GetComponent<Button>().onClick.AddListener(delegate { OnThumbnailClick(prefab.name); });
        tnPrefab.GetComponent<Button>().onClick.AddListener(delegate { addObject.onBtnClick(); });

        dict.Add(prefab.name, tnPrefab);
    }

    public void OnThumbnailClick(string name)
    {
        int id = int.Parse(name);
        ABUtils.Instance.OnChoosePrefab(id);
    }
}
