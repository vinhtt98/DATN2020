using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScrollPanel : MonoBehaviour
{
    [SerializeField]
    protected int rows = 1;
    [SerializeField]
    protected int columns = 1;
    [SerializeField]
    protected GameObject cellPrefab;
    protected Dictionary<string, GameObject> dict;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        dict = new Dictionary<string, GameObject>();
    }
    protected void setSize(int cellColumn)
    {
        if (transform.parent.GetComponent<ScrollRect>().horizontal)
        {
            float cellWidth = transform.parent.GetComponent<RectTransform>().sizeDelta.x / columns;
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(cellWidth * cellColumn, 0);
        }
        else
        {
            float cellHeight = transform.parent.GetComponent<RectTransform>().sizeDelta.y / rows;
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, cellHeight * cellColumn);
        }
    }
    public void makeScrollPanel()
    {
        int cellColumn = columns;
        if (dict.Count > rows * columns)
        {
            cellColumn = (dict.Count + rows - 1) / rows;
        }
        setSize(cellColumn);

        var list = dict.Keys.ToList();
        list.Sort();

        float width = transform.GetComponent<RectTransform>().rect.width;
        float height = transform.GetComponent<RectTransform>().rect.height;

        float deltaX = width / columns;
        float deltaY = height / rows;
        float startX = deltaX / 2;
        float startY = height / 2 - deltaY / 2;

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
}
