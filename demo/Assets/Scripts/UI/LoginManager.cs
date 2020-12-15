using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoginManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField loginField;
    [SerializeField]
    private TMP_InputField downloadField;
    [SerializeField]
    private Slider downloadSlider;
    [SerializeField]
    private GameObject blockInteration;

    private void Awake()
    {
        blockInteration.SetActive(false);
    }

    private void Start()
    {
        GameObject.FindObjectOfType<DownloadPanel>().loadDownloadedList();
    }

    public void OnLoginClick()
    {
        UserInfo.UserId = loginField.text;
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }

    public void OnDownloadClick()
    {
        blockInteration.SetActive(true);
        ABUtils.Instance.DownloadLink(downloadField.text, downloadSlider);
    }

    public void OnDownLoadComplete()
    {
        if (downloadSlider.value != downloadSlider.maxValue)
            return;
        blockInteration.SetActive(false);
        downloadSlider.value = 0;
        GameObject.FindObjectOfType<DownloadPanel>().loadDownloadedList();
    }
}
