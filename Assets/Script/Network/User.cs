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
    public bool isKeyHave;
    /// <summary>
    /// 열쇠조각 소유 수
    /// </summary>
    public int keyCount;
    /// <summary>
    /// 고양이 종류
    /// </summary>
    public int characterKind;
    /// <summary>
    /// 변신한 오브젝트 종류
    /// </summary>
    public int objectKind;

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

}
