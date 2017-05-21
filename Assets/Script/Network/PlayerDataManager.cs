using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 클라이언트의 정보를 가지고 있는 클래스
/// </summary>
public class PlayerDataManager : MonoBehaviour {

    public static PlayerDataManager instance;

    public User my;

    void Awake() {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            my = new User();
        }
        else
            Destroy(this.gameObject);
    }




}
