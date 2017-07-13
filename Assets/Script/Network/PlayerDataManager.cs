using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 클라이언트의 정보를 가지고 있는 클래스
/// </summary>
public class PlayerDataManager : MonoBehaviour {

    public static PlayerDataManager instance;

    public User my;
    public int itemType = 3;//소지 아이템 종류(0: 없음, 1: 은신물약, 2: 덫, 3: 변신물약)

    #region StateTimeVariable
    /// <summary>
    /// 은신 시간
    /// </summary>
    float hideTime = 0;

    /// <summary>
    /// 스턴 시간
    /// </summary>
    float stunTime = 0;

    /// <summary>
    /// 슬로우 시간
    /// </summary>
    float slowTime = 0;

    /// <summary>
    /// 변신 시간
    /// </summary>
    float changeTime = 0;
    #endregion

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

        if (stunTime > 0)
        {
            stunTime -= Time.deltaTime;
            if (stunTime <= 0)
            {
                Debug.Log("스턴 해제");
                my.PopState(User.State.Stun);
            }
        }

        if (slowTime > 0)
        {
            slowTime -= Time.deltaTime;
            if (slowTime <= 0)
            {
                Debug.Log("슬로우 해제");
                my.PopState(User.State.Slow);
            }
        }

        if(changeTime>0)
        {
            changeTime -= Time.deltaTime;
            if(changeTime<=0)
            {
                Debug.Log("변신 해제");
                my.PopState(User.State.Change);
                my.objectKind = 0;
            }
        }
    }

    #region ItemMethod
    /// <summary>
    /// 은신상태로 설정합니다.
    /// </summary>
    /// <param name="t">은신 시간</param>
    public void SetHide(float t)
    {
        Debug.Log("은신 시작");
        my.PushState(User.State.Hide);
        hideTime = t;
    }

    /// <summary>
    /// 스턴상태로 설정합니다.
    /// </summary>
    /// <param name="t">스턴 시간</param>
    public void SetStun(float t)
    {
        Debug.Log("스턴 시작");
        my.PushState(User.State.Stun);
        stunTime = t;
    }

    /// <summary>
    /// 슬로우상태로 설정합니다.
    /// </summary>
    /// <param name="t">슬로우 시간</param>
    public void SetSlow(float t)
    {
        Debug.Log("슬로우 시작");
        my.PushState(User.State.Slow);
        slowTime = t;
    }

    /// <summary>
    /// 변신상태로 설정합니다.
    /// </summary>
    /// <param name="t">변신 시간</param>
    /// <param name="objId">변신할 오브젝트 종류(1~3)</param>
    public void SetChange(float t,int objId)
    {
        my.objectKind = objId;//오브젝트종류먼저 설정해놔야됨!
        my.PushState(User.State.Change);
        changeTime = t;
    }

    /// <summary>
    /// 덫을 설치합니다.
    /// </summary>
    public void CreateTrap()
    {
        Debug.Log("트랩 설치");
        Trap t=Instantiate(GameManager.instance.blockObject[9], my.controller.transform.position, GameManager.instance.blockObject[9].transform.rotation).GetComponent<Trap>();
        t.SetOwner(my.name);
        NetworkManager.instance.SendTrap(t.transform.position);
    }
    #endregion


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
