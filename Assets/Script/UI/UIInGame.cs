using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInGame : MonoBehaviour
{
    public static UIInGame instance;

    public Text timer;

    public Transform canvas;

    public GameObject notice;
    public Text noticeText;
    public float noticeTime = 2f;

	public GameObject us,gs;

    public Sprite goodIcon, happyIcon, sadIcon, angryIcon, confuseIcon;

	public GameObject result;
	public Sprite loseResult;
	public Text resultText;
	public Text tagerText;
	public Text hiderText;
    public Text keyText;

    void Awake()
    {
        instance = this;
    }

    public void UpdateTimerText(string time)
    {
        timer.text = time;
    }

    public void TestEnter() {
    }

    public void SetHost() {
        PlayerDataManager.instance.my.isHost = true;
    }

    public void SetKeyValue(int current) {
        keyText.text = current + " / 3";
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


    public void SendEmoticon(int id) {
        NetworkManager.instance.SendChat(PlayerDataManager.instance.my.name,id.ToString(),2);
    }

    public Sprite GetEmotionIcon(int id) {
        switch (id) {
            case 0: return goodIcon;
            case 1: return happyIcon;
            case 2: return sadIcon;
            case 3: return angryIcon;
            case 4: return confuseIcon;
        }
        return null;
    }

	public void SetResult(bool lose,bool manWin)
	{
		if (lose) {
			result.GetComponent<Image> ().sprite = loseResult;
		}

		if(PlayerDataManager.instance.my.GetTeam())
			resultText.text="Tager";
		else
			resultText.text="Hider";

		GameObject[] ps = GameObject.FindGameObjectsWithTag ("Player");
		for (int i = 0; i < ps.Length; i++) {
			User u = ps [i].GetComponent<PlayerController> ().user;
			if (u.GetTeam ())
				tagerText.text += "\n"+u.name;
			else
				hiderText.text+= "\n"+u.name;
		}
		
		result.SetActive(true);
	}


    public void ExitGame() {
        NetworkManager.instance.SendExit();
    }

}
