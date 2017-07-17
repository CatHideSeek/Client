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

    public GameObject notice;
    public Text noticeText;
    public float noticeTime = 2f;


    void Awake()
    {
        instance = this;
    }

    public void UpdateTimerText(string time)
    {
        timer.text = time;
    }

    public void TestEnter() {
        NetworkManager.instance.TestEnterRoom();
		GameManager.instance.mapGenerator.InitMap ();
    }

    public void SetHost() {
        PlayerDataManager.instance.my.isHost = true;
    }

    //테스트 용 시작 버튼입니다. 언젠가 지워질꺼에요.
    public void TestStart()
    {
        PlayerDataManager.instance.my.isReady = true;
        NetworkManager.instance.enterRoom.SetFirstBoss();
        NetworkManager.instance.SendStart();
    }

    //테스트 용 레디 버튼입니다. 언젠가 지워질꺼에요.
    public void TestReady()
    {
        PlayerDataManager.instance.my.isReady = true;
        NetworkManager.instance.SendReady(true);
    }

    /// <summary>
    /// 공지 메세지를 출력합니다.
    /// </summary>
    /// <param name="message">메세지</param>
    public void ViewNotice(string message) {
        noticeText.text = message;
        StartCoroutine("WaitNotice");
    }

      
    public GameObject MakeNameLabel(Transform tr, string name) {
        GameObject g = Instantiate(nameLabel, canvas);

        g.GetComponent<UITargetUserInfo>().SetTarget(tr,name);

        return g;
    }


    IEnumerator WaitNotice() {
        notice.SetActive(true);
        yield return new WaitForSeconds(noticeTime);
        notice.SetActive(false);
    }

}
