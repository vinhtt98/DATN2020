using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
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
    [SerializeField]
    private GameObject deployParent;
    [SerializeField]
    private GameObject btn;
    private DatabaseReference mDatabaseRef;

    void Start()
    {
        // Get the root reference location of the database.
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void setBtn(bool enable)
    {
        btn.SetActive(enable);
    }

    public void OnConfirmClick()
    {
        ExportData data = new ExportData();
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


