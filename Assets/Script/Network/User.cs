using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// User 정보를 저장하는 클래스입니다.
/// </summary>
[System.Serializable]
public class User
{

    /// <summary>
    /// 유저 닉네임
    /// </summary>
    public string name;
    /// <summary>
    /// Socket ID
    /// </summary>
    public string socketID;

    /// <summary>
    /// 조종 가능한 플레이어 인가?
    /// </summary>
    public bool isPlayer;
    /// <summary>
    /// 방장 권한을 가지고 있는가?
    /// </summary>
    public bool isHost;
    /// <summary>
    /// 준비 완료 상태 인가?
    /// </summary>
    public bool isReady;
    /// <summary>
    /// 숙주 술래인가?
    /// </summary>
    public bool isBoss;
    /// <summary>
    /// 태그 당한 술래인가?
    /// </summary>
    public bool isBossChild;
    /// <summary>
    /// 키를 완성한 유저인가?
    /// </summary>
    public bool isKeyHave=false;
    /// <summary>
    /// 열쇠조각 소유 수
    /// </summary>
    public int keyCount=0;
    /// <summary>
    /// 고양이 종류
    /// </summary>
    public int characterKind;
    /// <summary>
    /// 변신한 오브젝트 종류
    /// </summary>
    public int objectKind=0;//0 변신안함, 1 나무, 2 수풀,3 돌

    /// <summary>
    /// 상태
    /// </summary>
    public List<State> states = new List<State>();

    /// <summary>
    /// 컨트롤러
    /// </summary>
    public PlayerController controller;

    public User()
    {
        this.name = "name";
    }

    public User(string name, string socketID)
    {
        this.name = name;
        this.socketID = socketID;
        this.isPlayer = false;
    }

    public enum State
    {
        Normal
            , Stun
            , Slow
            , Hide
            , Change
    }

    public int FindState(int state)
    {
        for (int i = 0; i < states.Count; i++)
        {
            if ((int)states[i] == state)
            {
                return i;
            }
        }
        return -1;
    }

    public void PushState(State state)
    {
        states.Add((State)state);
        if (PlayerDataManager.instance.my == this)
        {
            if (state == State.Change)
                NetworkManager.instance.SendState((int)state,objectKind);
            else
                NetworkManager.instance.SendState((int)state);
        }
    }

    public void PopState(State _state)
    {
        states.RemoveAll(delegate(State state){return state == _state;});
        if (PlayerDataManager.instance.my == this)
            NetworkManager.instance.SendState(-((int)_state));
    }

    public void EatKey()
    {
        keyCount++;
    }

    public void SetHaveKey() {
        isKeyHave = true;
    }

    public bool CheckTag() {
        if (isBoss || isBossChild)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 좀비인지 아닌지 반환해줍니다.(0:좀비아님, 1:좀비임)
    /// </summary>
    /// <returns></returns>
    public bool GetTeam()
    {
        return (isBoss || isBossChild);
    }
}
