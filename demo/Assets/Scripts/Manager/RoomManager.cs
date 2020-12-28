using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Firebase;
using Firebase.Database;

internal class ExportData
{
    public string userId { get; set; }
    public DateTime date { get; set; }
    public Dictionary<int, ObjectInfo> list { get; set; }
}

internal class ObjectInfo
{
    public int id { get; set; }
    public float width { get; set; }
    public float length { get; set; }
    public float height { get; set; }
    public ObjectInfo(int id, Vector3 vec)
    {
        this.id = id;
        this.length = vec.x;
        this.height = vec.y;
        this.width = vec.z;
    }
}
public class RoomManager : MonoBehaviour
{
    private enum ConfirmType
    {
        exit,
        upload
    }

    [SerializeField]
    private GameObject deployParent;
    [SerializeField]
    private GameObject confirmUploadUI;
    [SerializeField]
    private GameObject exitUI;
    [SerializeField]
    private CanvasGroup parentCanvasGroup;
    private DatabaseReference mDatabaseRef;
    public bool isEdit;
    public bool isOnExit;

    void Start()
    {
        // Get the root reference location of the database.
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        isEdit = false;
        isOnExit = false;
        addListener(confirmUploadUI.transform.GetChild(1).gameObject, ConfirmType.upload);
        addListener(exitUI.transform.GetChild(1).gameObject, ConfirmType.exit);
    }

    public void setBtn(bool enable)
    {
        GameObject btn = confirmUploadUI.transform.GetChild(0).gameObject;
        btn.SetActive(enable);
    }

    public void OnExitClick()
    {
        GameObject panel = exitUI.transform.GetChild(1).gameObject;
        panel.SetActive(true);
        parentCanvasGroup.blocksRaycasts = false;
    }

    public void OnConfirmClick()
    {
        GameObject panel = confirmUploadUI.transform.GetChild(1).gameObject;
        panel.SetActive(true);
        parentCanvasGroup.blocksRaycasts = false;
    }

    private void addListener(GameObject panel, ConfirmType type)
    {
        Button yes = panel.transform.GetChild(1).GetComponent<Button>();
        Button no = panel.transform.GetChild(2).GetComponent<Button>();
        yes.onClick.RemoveAllListeners();
        no.onClick.RemoveAllListeners();
        yes.onClick.AddListener(delegate { OnAcceptClick(type); });
        no.onClick.AddListener(delegate { OnDeclineClick(type); });
    }

    private void OnAcceptClick(ConfirmType type)
    {
        switch (type)
        {
            case ConfirmType.exit:
                if (isEdit)
                {
                    isOnExit = true;
                    exitUI.transform.GetChild(1).gameObject.SetActive(false);
                    confirmUploadUI.transform.GetChild(1).gameObject.SetActive(true);
                }
                else
                    SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
                break;
            case ConfirmType.upload:
                uploadData();
                isEdit = false;
                confirmUploadUI.transform.GetChild(1).gameObject.SetActive(false);
                if (isOnExit)
                    SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
                break;
        }
        parentCanvasGroup.blocksRaycasts = true;
    }

    private void OnDeclineClick(ConfirmType type)
    {
        switch (type)
        {
            case ConfirmType.exit:
                exitUI.transform.GetChild(1).gameObject.SetActive(false);
                break;
            case ConfirmType.upload:
                confirmUploadUI.transform.GetChild(1).gameObject.SetActive(false);
                if (isOnExit)
                    SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
                break;
        }
        parentCanvasGroup.blocksRaycasts = true;
    }

    private void uploadData()
    {
        ExportData data = new ExportData();
        if (UserInfo.UserId == null)
            UserInfo.UserId = "";
        data.userId = UserInfo.UserId;

        DateTimeOffset localTime = new DateTimeOffset(DateTime.UtcNow);
        data.date = localTime.ToOffset(new TimeSpan(7, 0, 0)).DateTime;
        data.list = new Dictionary<int, ObjectInfo>();

        for (int i = 0; i < deployParent.transform.childCount; i++)
        {
            // Debug.Log(i + " " + deployParent.transform.GetChild(i).name);
            Transform transform = deployParent.transform.GetChild(i);
            int id = int.Parse(transform.name);

            DeployObjectProperty property;
            if (ABUtils.goPropDict.TryGetValue(id, out property))
            {
                ObjectInfo dimension = new ObjectInfo(id, Vector3.Scale(property.objectSize, transform.localScale));
                data.list.Add(i, dimension);
            }
        }

        string json = JsonConvert.SerializeObject(data);

        mDatabaseRef.Child("database").Push().SetRawJsonValueAsync(json);

        Debug.Log(json);
    }
}


