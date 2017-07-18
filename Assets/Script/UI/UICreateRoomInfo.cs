using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UICreateRoomInfo : MonoBehaviour
{

    public InputField roomNameInput, roomPwInput;
    public Dropdown maxPlayersDropDown;


    public void OnCreateRoom()
    {
        string name = roomNameInput.text;

        if (name == "")
            return;

        int pw = -1;
        if (roomPwInput.text != "")
             pw = int.Parse(roomPwInput.text);
        int maxPlayer = 8;
        switch (maxPlayersDropDown.value)
        {
            case 0: maxPlayer = 8; break;
            case 1: maxPlayer = 7; break;
            case 2: maxPlayer = 6; break;
            case 3: maxPlayer = 5; break;
            case 4: maxPlayer = 4; break;
            case 5: maxPlayer = 3; break;
            case 6: maxPlayer = 2; break;
            default: break;
        }

        NetworkManager.instance.SendRoomCreate(name, maxPlayer, pw);
    }


}
