using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomSlot : MonoBehaviour {

    public Text numberText, nameText, countText, stateText;

    public void SetData(int index,Room room) {
        numberText.text = "No. " + index;
        nameText.text = room.name;
        countText.text = room.countPlayers + " / " + room.maxPlayers;
        if (room.isPlay)
            stateText.text = "<color=#ff0000ff>Playing..</color>";
        else if (room.countPlayers == room.maxPlayers)
            stateText.text = "<color=#00ffffff>Full..</color>";
        else
            stateText.text = "<color=#00ff00ff>Wait..</color>";
    }

}
