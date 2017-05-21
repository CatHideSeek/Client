using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room
{

    public int id;
    public string name;

    public int countPlayers;
    public int readyPlayers;
    public int maxPlayers;
    public List<User> userList = new List<User>();

    public bool isPlay;

    public Room()
    {
        this.id = 0;
        this.name = "Test";
        this.maxPlayers = 2;
    }

    public Room(int id, string name, int fullPlayer)
    {
        this.id = id;
        this.name = name;
        this.maxPlayers = fullPlayer;
    }

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

    public void DeletUser(string name)
    {
        for (int i = 0; i < userList.Count; i++)
        {
            if (userList[i].name == name)
            {
                userList.RemoveAt(i);
                break;
            }
        }
    }

}
