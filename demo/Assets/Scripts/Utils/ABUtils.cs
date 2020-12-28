using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime;
using UnityEngine.UI;
using Newtonsoft.Json;

public class DeployObjectProperty
{
    public int id { get; set; }
    public string tag { get; set; }
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
    public static ConcurrentDictionary<int, string> tagDict;
    public enum DownloadType
    {
        SaveLocal,
        LoadLocal
    }
    private const float delayValue = 0.5f;
    private void Awake()
    {
        cachedList = new List<CachedAssetBundle>();
        goDict = new ConcurrentDictionary<int, GameObject>();
        goPropDict = new ConcurrentDictionary<int, DeployObjectProperty>();
        tagDict = new ConcurrentDictionary<int, string>();

        string cachedPath = Path.Combine(Application.persistentDataPath, "CachedAssetBundleList.json");
        try
        {
            var sr = new StreamReader(cachedPath);
            var fileContents = sr.ReadToEnd();
            sr.Close();
            cachedList = JsonConvert.DeserializeObject<List<CachedAssetBundle>>(fileContents);
            if (cachedList == null)
            {
                cachedList = new List<CachedAssetBundle>();
                File.WriteAllText(cachedPath, "");
                Debug.Log("File CachedAssetBundleList.json created");
            }

        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            File.WriteAllText(cachedPath, "");
            Debug.Log("File CachedAssetBundleList.json created");
        }

        // Debug.Log("Init ABUtils");
        // Debug.Log("Init cachedList.Count" + cachedList.Count);

        // DownloadLink("https://drive.google.com/uc?export=download&id=1f2dmogGDfDwKs5J-S1-FPmvaiMeQO6pJ", null);
        // RemoveLocal("https://drive.google.com/uc?export=download&id=1f2dmogGDfDwKs5J-S1-FPmvaiMeQO6pJ");
        // Caching.ClearAllCachedVersions("default");
    }

    public void DownloadLink(string url, Slider slider)
    {
        if (slider != null)
        {
            slider.maxValue = 1 + delayValue;
        }

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

    public void LoadAllAsset(Slider slider)
    {
        Debug.Log("Load all");

        if (slider != null)
        {
            Debug.Log("slider.maxValue" + slider.maxValue);
            Debug.Log("cachedList.Count" + cachedList.Count);
            Debug.Log("delayValue" + delayValue);

            slider.maxValue = (cachedList.Count + 1) * (1 + delayValue);
        }

        string defaultPath = Path.Combine((Application.platform == RuntimePlatform.Android ? "" : "file://") + Application.streamingAssetsPath, "default");
        StartCoroutine(DownloadAsset(defaultPath, 0, DownloadType.LoadLocal, slider));

        foreach (CachedAssetBundle entry in cachedList)
        {
            StartCoroutine(DownloadAsset(entry.url, entry.version, DownloadType.LoadLocal, slider));
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
                    slider.value += request.downloadProgress - prevProgres;
                prevProgres = request.downloadProgress;
                Debug.Log("Downloading: " + url + "\nVersion: " + version + "\nProgress: " + request.downloadProgress * 100f);
                yield return null;
            }
            if (slider != null)
                slider.value += 1 - prevProgres;
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
                    bundle.Unload(false);
                }
            }
            if (slider != null)
                slider.value += delayValue;
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
        // Debug.Log(json);

        string savePath = Path.Combine(Application.persistentDataPath, "CachedAssetBundleList.json");
        File.WriteAllText(savePath, json);
    }

    private static void LoadLocal(AssetBundle bundle)
    {
        //Load all properties
        TextAsset config = bundle.LoadAsset("config") as TextAsset;
        Debug.Log(config.text);

        List<DeployObjectProperty> properties = JsonConvert.DeserializeObject<List<DeployObjectProperty>>(config.text);
        foreach (DeployObjectProperty property in properties)
        {
            var prefab = bundle.LoadAsset<GameObject>(property.id.ToString());
            prefab.name = property.id.ToString();
            goDict.TryAdd(property.id, prefab);
            goPropDict.TryAdd(property.id, property);
            tagDict.TryAdd(property.id, property.tag);

            // Debug.Log(prefab.name);
        }
    }

    public static void RemoveLocal(string url)
    {
        foreach (CachedAssetBundle entry in cachedList)
        {
            if (entry.url.Equals(url))
            {
                Caching.ClearAllCachedVersions(entry.name);

                Uri uri = new Uri(url);
                string bundle_name = System.IO.Path.GetFileNameWithoutExtension(uri.AbsolutePath);
                Caching.ClearAllCachedVersions(bundle_name);

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
            DeployObjectProperty property;
            if (goPropDict.TryGetValue(id, out property))
            {
                objectManager.setTargetGameObject(null);
                objectManager.deployObject(prefab, property);
            }
        }
        else
        {
            Debug.Log("Wrong id: " + id);
        }
    }
}