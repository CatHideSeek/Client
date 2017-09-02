using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomSlot : MonoBehaviour {

    public Text numberText, nameText, countText, stateText;
    public GameObject lockIcon;
    Button slotButton;

    private Room roomData;

    public bool isWaitRoom = false;

    private void Awake()
    {
        slotButton = GetComponent<Button>();
    }


    public void SetData(int index,Room room) {
        roomData = room;

        
        nameText.text = room.name;
        countText.text = room.countPlayers + " / " + room.maxPlayers;

        if (room.pw != -1) {
            lockIcon.SetActive(true);
        }
        else
            lockIcon.SetActive(false);

        room.ui = this;

        if (!isWaitRoom)
        {
            numberText.text = "No. " + index;
            if (room.isPlay)
            {
                stateText.text = "<color=#ff0000ff>Playing..</color>";
                slotButton.enabled = false;
            }
            else if (room.countPlayers == room.maxPlayers)
            {
                stateText.text = "<color=#00ffffff>Full..</color>";
                slotButton.enabled = false;
            }
            else
            {
                stateText.text = "<color=#00ff00ff>Wait..</color>";
                slotButton.enabled = true;
            }
        }
    }

    public void SetData(Room room)
    {
        roomData = room;

        nameText.text = room.name;
        countText.text = room.countPlayers + " / " + room.maxPlayers;

        if (room.pw != -1)
        {
            lockIcon.SetActive(true);
        }
        else
            lockIcon.SetActive(false);

        if (!isWaitRoom)
        {

            if (room.isPlay)
            {
                stateText.text = "<color=#ff0000ff>Playing..</color>";
                slotButton.enabled = false;
            }
            else if (room.countPlayers == room.maxPlayers)
            {
                stateText.text = "<color=#00ffffff>Full..</color>";
                slotButton.enabled = false;
            }
            else
            {
                stateText.text = "<color=#00ff00ff>Wait..</color>";
                slotButton.enabled = true;
            }
        }
    }



    public void OnEnterRoom()
    {
        SoundManager.instance.PlayButtonBGS();
        if (roomData.pw != -1) {
            GameObject.FindWithTag("UIManager").GetComponent<UILobby>().SetPwInput(roomData);
        }
        else
        {
            NetworkManager.instance.SendWaitRoomEnter(roomData.id + roomData.name);
        }
    }

    public void OnEnterRoom(bool pass)
    {
        if (pass)
            NetworkManager.instance.SendWaitRoomEnter(roomData.id + roomData.name);
        else
            GameObject.FindWithTag("UIManager").GetComponent<UILobby>().SetWrongPw();
    }
    

}
