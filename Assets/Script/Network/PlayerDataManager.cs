using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 클라이언트의 정보를 가지고 있는 클래스
/// </summary>
public class PlayerDataManager : MonoBehaviour {

    public static PlayerDataManager instance;

    public User my;
    public int itemType = 0;//소지 아이템 종류(0: 없음, 1: 은신물약, 2: 덫)
  
    float hideTime=0;

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

    void Update()
    {
        if(hideTime>0)
        {
            hideTime -= Time.deltaTime;
            if (hideTime <= 0)
            {
                Debug.Log("은신 해제");
                my.PopState(User.State.Hide);
            }
        }
    }

    public void SetHide(float t)
    {
        Debug.Log("은신 시작");
        my.PushState(User.State.Hide);
        hideTime = t;
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

    public void EatItem(int id)
    {
        itemType = id;
    }
}
