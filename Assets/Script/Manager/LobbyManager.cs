using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        NetworkManager.instance.SendLobbyEnter();
	}
}
