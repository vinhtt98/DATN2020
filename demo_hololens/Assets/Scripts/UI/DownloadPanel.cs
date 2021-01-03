using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DownloadPanel : MonoBehaviour
{
    [SerializeField]
    protected int rows = 1;
    [SerializeField]
    protected int columns = 1;
    [SerializeField]
    protected GameObject cellPrefab;
    protected Dictionary<string, GameObject> dict;
    protected void Awake()
    {
        dict = new Dictionary<string, GameObject>();

    }

    public void loadDownloadedList()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        dict.Clear();

        foreach (CachedAssetBundle cached in ABUtils.cachedList)
        {
            GameObject prefab = Instantiate(cellPrefab, Vector3.zero, Quaternion.identity, this.transform);

            TextMeshProUGUI name = prefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI version = prefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI url = prefab.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

            Button btn = prefab.transform.GetChild(3).GetComponent<Button>();

            name.text += cached.name;
            version.text += cached.version;
            url.text += cached.url;

            btn.onClick.AddListener(delegate { OnRemoveClick(cached.url); });

            dict.Add(cached.name, prefab);
        }

        makeScrollPanel();
    }

    protected void setSize(int cellColumn)
    {
        float cellHeight = transform.parent.GetComponent<RectTransform>().rect.height / rows;
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, cellHeight * cellColumn);
    }
    public void makeScrollPanel()
    {
        int cellColumn = rows;

        if (dict.Count > rows * columns)
        {
            cellColumn = (dict.Count + rows - 1) / rows;
        }
        setSize(cellColumn);

        var list = dict.Keys.ToList();
        list.Sort();

        // Debug.Log(transform.parent.GetComponent<RectTransform>().rect);
        // Debug.Log(transform.GetComponent<RectTransform>().rect);

        float width = transform.GetComponent<RectTransform>().rect.width;
        float height = transform.GetComponent<RectTransform>().rect.height;

        float deltaX = width / columns;
        float deltaY = height / rows;
        float startX = 0;
        float startY = -deltaY / 2;

        int i = 0;
        foreach (var key in list)
        {
            Vector3 pos;
            if (i < rows * columns)
            {
                pos = new Vector3(startX + deltaX * (i % columns), startY - deltaY * (i / columns), 0);
            }
            else
            {
                pos = new Vector3(startX + deltaX * (i / rows), startY - deltaY * (i % rows), 0);
            }
            dict[key].transform.localPosition = pos;
            i++;
        }
    }

    public void OnRemoveClick(string url)
    {
        ABUtils.RemoveLocal(url);
        this.loadDownloadedList();
    }
}
