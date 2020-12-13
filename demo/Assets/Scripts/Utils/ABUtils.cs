using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;

public class DeployObjectProperty
{
    public int id { get; set; }
    public float width { get; set; }
    public float length { get; set; }
    public float height { get; set; }
    public bool isCanResize { get; set; }
    public bool isOnVerticalPlane { get; set; }
    public bool isOnCeil { get; set; }
    public Vector3 objectSize
    {
        get { return new Vector3(length, height, width); }
    }
}

public class CachedAssetBundle
{
    public string name { get; set; }
    public string url { get; set; }
    public uint version { get; set; }
    public CachedAssetBundle(string name, string url, uint version)
    {
        this.name = name;
        this.url = url;
        this.version = version;
    }
}

public class ABUtils : MonoBehaviour
{
    #region SINGLETON PATTERN
    private static ABUtils instance = null;
    public static ABUtils Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<ABUtils>();

                if (instance == null)
                {
                    GameObject container = new GameObject("ABUtils");
                    instance = container.AddComponent<ABUtils>();
                }
            }
            return instance;
        }
    }
    #endregion
    public static List<CachedAssetBundle> cachedList;
    public static ConcurrentDictionary<int, GameObject> goDict;
    public static ConcurrentDictionary<int, DeployObjectProperty> goPropDict;
    public enum DownloadType
    {
        SaveLocal,
        LoadLocal
    }
    private void Awake()
    {
        cachedList = new List<CachedAssetBundle>();
        goDict = new ConcurrentDictionary<int, GameObject>();
        goPropDict = new ConcurrentDictionary<int, DeployObjectProperty>();

        string cachedPath = Path.Combine(Application.persistentDataPath, "CachedAssetBundleList.json");
        try
        {
            var sr = new StreamReader(cachedPath);
            var fileContents = sr.ReadToEnd();
            sr.Close();
            cachedList = JsonConvert.DeserializeObject<List<CachedAssetBundle>>(fileContents);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            File.WriteAllText(cachedPath, "");
        }

        // DownloadLink("https://drive.google.com/uc?export=download&id=1f2dmogGDfDwKs5J-S1-FPmvaiMeQO6pJ", null);
        // RemoveLocal("https://drive.google.com/uc?export=download&id=1f2dmogGDfDwKs5J-S1-FPmvaiMeQO6pJ");
        LoadAllAsset();
    }

    public void DownloadLink(string url, Slider slider)
    {
        foreach (CachedAssetBundle entry in cachedList)
        {
            if (entry.url.Equals(url))
            {
                StartCoroutine(DownloadAsset(url, entry.version + 1, DownloadType.SaveLocal, slider));
                return;
            }
        }
        StartCoroutine(DownloadAsset(url, 1, DownloadType.SaveLocal, slider));
    }

    public void LoadAllAsset()
    {
        string defaultPath = Path.Combine((Application.platform == RuntimePlatform.Android ? "" : "file://") + Application.streamingAssetsPath, "default");
        StartCoroutine(DownloadAsset(defaultPath, 0, DownloadType.LoadLocal, null));

        foreach (CachedAssetBundle entry in cachedList)
        {
            StartCoroutine(DownloadAsset(entry.url, entry.version, DownloadType.LoadLocal, null));
        }
    }

    private static IEnumerator DownloadAsset(string url, uint version, DownloadType type, Slider slider)
    {
        //string url = "file:///" + Application.persistentDataPath + "/AssetBundles/" + assetBundleName;
        using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url, version, 0))
        {
            var operation = request.SendWebRequest();
            float prevProgres = 0;
            while (!operation.isDone)
            {
                /* 
                 * as BugFinder metnioned in the comments
                 * what you want to track is uwr.downloadProgress
                 */

                /*
                 * use a float division here 
                 * I don't know what type downloadDataProgress is
                 * but if it is an int than you will always get 
                 * an int division <somethingSmallerThan100>/100 = 0
                 */
                if (slider != null)
                    slider.value = request.downloadProgress - prevProgres;
                prevProgres = request.downloadProgress;
                Debug.Log("Downloading: " + url + "\nVersion: " + version + "\nProgress: " + request.downloadProgress * 100f);
                yield return null;
            }
            if (slider != null)
                slider.value = 1;
            Debug.Log("Downloading: " + url + "\nVersion: " + version + "\nProgress: Finished!");
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error + " " + url);
            }
            else
            {
                // Get downloaded asset bundle
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

                if (bundle != null)
                {
                    switch (type)
                    {
                        case DownloadType.SaveLocal:
                            SaveLocal(bundle.name, url, version);
                            break;
                        case DownloadType.LoadLocal:
                            LoadLocal(bundle);
                            break;
                    }
                }
            }
        }
    }

    private static void SaveLocal(string name, string url, uint version)
    {
        CachedAssetBundle newEntry = new CachedAssetBundle(name, url, version);
        foreach (CachedAssetBundle entry in cachedList)
        {
            if (entry.url.Equals(url))
            {
                entry.name = name;
                entry.version = version;
                newEntry = null;
                break;
            }
        }
        if (newEntry != null)
            cachedList.Add(newEntry);

        string json = JsonConvert.SerializeObject(cachedList);

        string savePath = Path.Combine(Application.persistentDataPath, "CachedAssetBundleList.json");
        File.WriteAllText(savePath, json);
    }

    private static void LoadLocal(AssetBundle bundle)
    {
        //Load all properties
        TextAsset config = bundle.LoadAsset("config") as TextAsset;

        List<DeployObjectProperty> properties = JsonConvert.DeserializeObject<List<DeployObjectProperty>>(config.text);
        foreach (DeployObjectProperty property in properties)
        {
            var prefab = bundle.LoadAsset<GameObject>(property.id.ToString());
            prefab.name = property.id.ToString();
            goDict.TryAdd(property.id, prefab);
            goPropDict.TryAdd(property.id, property);

            Debug.Log(prefab.name);
        }

        bundle.Unload(false);
    }

    private static void RemoveLocal(string url)
    {
        foreach (CachedAssetBundle entry in cachedList)
        {
            if (entry.url.Equals(url))
            {
                cachedList.Remove(entry);
                break;
            }
        }

        string json = JsonConvert.SerializeObject(cachedList);

        string savePath = Path.Combine(Application.persistentDataPath, "CachedAssetBundleList.json");
        File.WriteAllText(savePath, json);
    }

    public void OnChoosePrefab(int id)
    {
        ObjectManager objectManager = GameObject.Find("GameManager").GetComponent<ObjectManager>();
        GameObject prefab;
        if (goDict.TryGetValue(id, out prefab))
        {
            objectManager.deployObject(prefab.name, prefab, new Vector3(0, 0, 5));
        }
        else
        {
            Debug.Log("Wrong id: " + id);
        }
    }
}