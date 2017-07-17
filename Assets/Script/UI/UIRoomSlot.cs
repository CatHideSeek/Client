using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomSlot : MonoBehaviour {

    public Text numberText, nameText, countText, stateText;
    Button slotButton;

    private Room roomData;

    private void Awake()
    {
        slotButton = GetComponent<Button>();
    }


    public void SetData(int index,Room room) {
        roomData = room;

        numberText.text = "No. " + index;
        nameText.text = room.name;
        countText.text = room.countPlayers + " / " + room.maxPlayers;

        room.ui = this;

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

    public void SetData(Room room)
    {
        roomData = room;

        nameText.text = room.name;
        countText.text = room.countPlayers + " / " + room.maxPlayers;

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



    public void OnEnterRoom() {
        NetworkManager.instance.SendEnter(roomData.id+roomData.name);
    }



}
