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


    public void EatKey()
    {
        print("열쇠 소지갯수: " + my.keyCount);
        NetworkManager.instance.SendGetKeyPart(my.keyCount);

        if (my.keyCount + 1 >= 4) {
            my.isKeyHave = true;
            NetworkManager.instance.SendGetKey(my.isKeyHave);
        }
    }
}
