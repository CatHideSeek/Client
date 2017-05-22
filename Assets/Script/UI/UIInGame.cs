using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInGame : MonoBehaviour
{
    public static UIInGame instance;

    public Text timer;

    public Transform canvas;
    public GameObject nameLabel;



    void Awake()
    {
        instance = this;
    }

    public void UpdateTimerText(string time)
    {
        timer.text = time;
    }

    //테스트 용 레디 버튼입니다. 언젠가 지워질꺼에요.
    public void TestStart()
    {
        PlayerDataManager.instance.my.isReady = true;
        NetworkManager.instance.SendStart();
    }

    //테스트 용 레디 버튼입니다. 언젠가 지워질꺼에요.
    public void TestReady()
    {
        PlayerDataManager.instance.my.isReady = true;
        NetworkManager.instance.SendReady(true);
    }

    public void MakeNameLabel(Transform tr, string name) {
        GameObject g = Instantiate(nameLabel, canvas);

        g.GetComponent<UITargetUserName>().SetTarget(tr,name);
    }

}
