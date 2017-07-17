using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 타이틀 씬에 관한 모든 UI를 제어하는 클래스입니다.
/// </summary>
public class UITitle : MonoBehaviour
{


    public InputField nickInput;
    public GameObject nameError;

    public void OnJoin()
    {
        if (nickInput.text != "")
            NetworkManager.instance.SendRegister(nickInput.text);
    }




}
