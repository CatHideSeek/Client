using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPWInput : MonoBehaviour {


    Room room;

    public void SetRoomData(Room data) {
        room = data;
    }

    public void CheckPW(InputField pw) {
        room.ui.OnEnterRoom(room.CheckPW(int.Parse(pw.text)));
    }



}
