using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 타이틀 씬에 관한 모든 UI를 제어하는 클래스입니다.
/// </summary>
public class UITitle : MonoBehaviour
{

    public PortalController portal;

    public InputField nickInput;
    public GameObject nameError;

    public void OnJoin()
    {
        SoundManager.instance.PlayButtonBGS();
        if (nickInput.text != "")
            NetworkManager.instance.SendRegister(nickInput.text);
    }

    void Start() {
        SoundManager.instance.PlayTitleBGM();
        StartCoroutine(TitleAnimation());
    }
    

    IEnumerator TitleAnimation() {
        float times = 0;
        while (true) {
            if ((int)times == 10)
            {
                portal.Open();
            }
            else if ((int)times == 20) {
                portal.CloseScrue();
                times = 0;
            }

            times += Time.deltaTime;
            yield return null;
        }
    }



}
