using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitRoomManager : MonoBehaviour {
    void Start() {
        NetworkManager.instance.SendWaitRoomJoin();
        SoundManager.instance.PlayWaitRoomBGM();
    }
}
