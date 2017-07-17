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

	public GameObject us,gs;

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
		Debug.Log ("sdfdsfds");
        noticeText.text = message;
        StartCoroutine("WaitNotice");
    }

	public void ViewUserState(string message){
		us.GetComponent<StateAlarm> ().PushAlarm (message);
		StartCoroutine("WaitUserState");
	}

	public void ViewGameState(string message)
	{
		gs.GetComponent<StateAlarm> ().PushAlarm (message);
		StartCoroutine("WaitGameState");
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

	IEnumerator WaitUserState() {
		us.SetActive(true);
		yield return new WaitForSeconds(noticeTime);
		us.SetActive(false);
	}

	IEnumerator WaitGameState() {
		gs.SetActive(true);
		yield return new WaitForSeconds(noticeTime);
		gs.SetActive(false);
	}
}
