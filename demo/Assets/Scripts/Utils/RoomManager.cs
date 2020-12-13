using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Newtonsoft.Json;

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
            Debug.Log(i + " " + deployParent.transform.GetChild(i).name);
            int id = int.Parse(deployParent.transform.GetChild(i).name);

            DeployObjectProperty property;
            if (ABUtils.goPropDict.TryGetValue(id, out property))
            {
                ObjectInfo dimension = new ObjectInfo(id, property.objectSize);
                data.list.Add(i, dimension);
            }
        }

        string json = JsonConvert.SerializeObject(data);

        Debug.Log(json);
    }
}


