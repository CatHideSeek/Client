using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SceneLoadManager : MonoBehaviour {

    public static SceneLoadManager instance;

    [HideInInspector]
    public UnityEvent sceneMethod = null;

    void Awake() {
        if (instance != null)
            Destroy(this.gameObject);
        else {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    /// <summary>
    /// 씬을 로드 하는 함수. 
    /// </summary>
    /// <param name="scene">On(SCENE_NAME)의 함수를 넣어주세요.</param>
    public void LoadScene(UnityAction scene) {
        sceneMethod.AddListener(scene);
        SceneManager.LoadScene("LoadScene");
    }

    /// <summary>
    /// LoadScene 함수의 인자가 되는 함수, 로비로 이동합니다.
    /// </summary>
    public void OnLobby() {
        SceneManager.LoadScene("LobbyScene");
    }
    /// <summary>
    /// LoadScene 함수의 인자가 되는 함수, 대기실로 이동합니다.
    /// </summary>
    public void OnWaitRoom() {
        SceneManager.LoadScene("WaitScene");
    }
    /// <summary>
    /// LoadScene 함수의 인자가 되는 함수, 인게임으로 이동합니다.
    /// </summary>
    public void OnInGame() {
        SceneManager.LoadScene("InGameScene");
    }

}
