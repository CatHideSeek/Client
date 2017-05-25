using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 방에 대한 정보와 함수를 가진 클래스 입니다.
/// </summary>
[System.Serializable]
public class Room
{
    /// <summary>
    /// 방의 아이디
    /// </summary>
    public int id;
    /// <summary>
    /// 방의 이름
    /// </summary>
    public string name;
    /// <summary>
    /// 방 내의 사람 수
    /// </summary>
    public int countPlayers;
    /// <summary>
    /// 방 내의 준비 된 수
    /// </summary>
    public int readyPlayers;
    /// <summary>
    /// 방의 최대 입장 가능 수
    /// </summary>
    public int maxPlayers;
    /// <summary>
    /// 방 내 유저 리스트
    /// </summary>
    public List<User> userList = new List<User>();
    /// <summary>
    /// 방이 게임 플레이 중 인가?
    /// </summary>
    public bool isPlay;
    /// <summary>
    /// 방의 술래가 된 유저 리스트
    /// </summary>
    public int bossPlayers = 0;

    /// <summary>
    /// [TEST] 방의 초기값 생성
    /// </summary>
    public Room()
    {
        this.id = 0;
        this.name = "Test";
        this.maxPlayers = 2;
        this.userList = new List<User>();
    }
    
    /// <summary>
    /// 방을 생성합니다.
    /// </summary>
    /// <param name="id">방 아이디</param>
    /// <param name="name">방 이름</param>
    /// <param name="fullPlayer">최대 인원 수</param>
    public Room(int id, string name, int fullPlayer)
    {
        this.id = id;
        this.name = name;
        this.maxPlayers = fullPlayer;
        this.userList = new List<User>();
    }

    /// <summary>
    /// 방 내 유저 리스트 에서 특정 이름을 가진 유저를 찾습니다.
    /// </summary>
    /// <param name="name">유저 이름</param>
    /// <returns></returns>
    public User FindUserByName(string name)
    {
        for (int i = 0; i < userList.Count; i++)
        {
            if (userList[i].name == name)
            {
                return userList[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 방 내 유저 리스트 에서 특정 Socket ID 를 가진 유저를 찾습니다.
    /// </summary>
    /// <param name="socketID">Socket ID</param>
    /// <returns></returns>
    public User FindUserBySocketID(string socketID)
    {
        for (int i = 0; i < userList.Count; i++)
        {
            if (userList[i].socketID == socketID)
            {
                return userList[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 방 내 유저 리스트에 특정 유저를 추가 합니다.
    /// </summary>
    /// <param name="user"></param>
    public void AddUser(User user) {
        userList.Add(user);
        UpdateCountPlayer(1);
    }

    /// <summary>
    /// 방 내 유저 리스트에 특정 유저를 지웁니다.
    /// </summary>
    /// <param name="name">유저 이름</param>
    public void DeletUser(string name)
    {
        for (int i = 0; i < userList.Count; i++)
        {
            if (userList[i].name == name)
            {
                userList[i].controller.DeletUser();

                if (userList[i].isBoss || userList[i].isBossChild)
                    UpdateBossPlayer(-1);

                UpdateCountPlayer(-1);
                userList.RemoveAt(i);
                break;
            }
        }
    }

    /// <summary>
    /// 방 내 유저 리스트에서 숙주 술래가 되는 유저를 선택하고 전송합니다.
    /// </summary>
    public void SetFirstBoss() {
        int rand = Random.Range(0,countPlayers);
        NetworkManager.instance.SendRootTag(userList[rand].name);
    }

    /// <summary>
    /// 방 내 유저 수를 조정합니다.
    /// </summary>
    /// <param name="count">카운트 + / - </param>
    public void UpdateCountPlayer(int count) {
        countPlayers += count;
    }

    /// <summary>
    /// 술래의 수를 업데이트 합니다.
    /// </summary>
    /// <param name="count">카운트 + / - </param>
    public void UpdateBossPlayer(int count)
    {
        if (!GameManager.instance.isEnd)
        {
            Debug.Log("업데이트 : " + count);
            bossPlayers += count;
            if (bossPlayers == countPlayers)
            {
                Debug.Log("게임 종료 술래 승리");
                GameManager.instance.isEnd = true;
            }
        }
    }

}
