using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 타이틀 씬에 관한 모든 UI를 제어하는 클래스입니다.
/// </summary>
public class UITitle : MonoBehaviour {

    public static UITitle instance;

    public InputField idInput, pwInput, nickInput, emailInput;


    private void Awake()
    {
        instance = this;
    }



    public void StartButton() {
        SceneLoadManager.instance.LoadScene(SceneLoadManager.instance.OnLobby);
    }

    public void LoginButton() {

    }

    public void RegisterButton() {

    }

    public void SendRegister() {

    }



}
