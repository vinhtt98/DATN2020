using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal class TabInfo
{
    public GameObject tab;
    public GameObject page;
    public TabInfo(GameObject tab, GameObject page)
    {
        this.tab = tab;
        this.page = page;
    }
}
public class AddPanel : MonoBehaviour
{
    [SerializeField]
    private AddPanelTab addPanelTab;
    [SerializeField]
    private GameObject pageArea;
    public GameObject addPanelPagePrefab;
    private Dictionary<string, TabInfo> tabInfoDict;

    // Start is called before the first frame update
    void Start()
    {
        tabInfoDict = new Dictionary<string, TabInfo>();
    }
    public void addPrefab(string tabName, GameObject prefab)
    {
        TabInfo tabInfo;
        if (tabInfoDict.TryGetValue(tabName, out tabInfo))
        {
            tabInfo.page.GetComponent<AddPanelPage>().addThumbnail(prefab);
        }
        else
        {
            GameObject tab = addPanelTab.addTab(tabName);
            GameObject page = Instantiate(addPanelPagePrefab, Vector3.zero, Quaternion.identity, pageArea.transform);

            page.transform.localPosition = new Vector3(-pageArea.GetComponent<RectTransform>().rect.width / 2, 0, 0);
            page.GetComponent<AddPanelPage>().addThumbnail(prefab);

            tabInfo = new TabInfo(tab, page);
            tabInfoDict.Add(tabName, tabInfo);
        }
    }

    public void switchTab(GameObject tab)
    {
        foreach (TabInfo tabInfo in tabInfoDict.Values)
        {
            if (tabInfo.tab.GetInstanceID() == tab.GetInstanceID()) {
                tabInfo.page.SetActive(true);
                pageArea.GetComponent<ScrollRect>().content = tabInfo.page.GetComponent<RectTransform>();
            } else {
                tabInfo.page.SetActive(false);
            }
        }
    }

    public void makeAddPanel() {
        int i = 0;
        TabInfo firstTabInfo = null;
        foreach (KeyValuePair<string,TabInfo> entry in tabInfoDict) {
            if (i==0) {
                firstTabInfo = entry.Value;
            } else {
                if (string.Compare(firstTabInfo.tab.name, entry.Key) > 0) {
                    firstTabInfo = entry.Value;
                }
            }

            entry.Value.page.GetComponent<AddPanelPage>().makeScrollPanel();

            entry.Value.page.SetActive(false);
            i++;
        }

        addPanelTab.makeScrollPanel();

        switchTab(firstTabInfo.tab);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
