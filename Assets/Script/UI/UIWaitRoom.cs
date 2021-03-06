﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWaitRoom : MonoBehaviour
{

    public UIRoomSlot roomInfo;

    public Text chatLog, readyText;

    [SerializeField]
    bool isReady = false;

    public void SetRoomInfo(Room room)
    {
        roomInfo.SetData(room);
        if (PlayerDataManager.instance.my.isHost)
        {
            if (NetworkManager.instance.enterRoom.readyPlayers >= 2 && NetworkManager.instance.enterRoom.readyPlayers == NetworkManager.instance.enterRoom.countPlayers)
                readyText.text = "시작\n" + room.readyPlayers + "/" + room.maxPlayers;
            else
                readyText.text = "대기\n" + room.readyPlayers + "/" + room.maxPlayers;
        }
        else {
            readyText.text = "준비\n" + room.readyPlayers + "/" + room.maxPlayers;
        }
    }

    public void UpdateReadyText(Room room)
    {
        roomInfo.SetData(room);
        if (PlayerDataManager.instance.my.isHost)
        {
            if (NetworkManager.instance.enterRoom.readyPlayers >= 2 && NetworkManager.instance.enterRoom.readyPlayers == NetworkManager.instance.enterRoom.countPlayers)
                readyText.text = "시작\n" + room.readyPlayers + "/" + room.maxPlayers;
            else
                readyText.text = "대기\n" + room.readyPlayers + "/" + room.maxPlayers;
        }
        else {
            readyText.text = "준비\n" + room.readyPlayers + "/" + room.maxPlayers;
        }
    }

    public void SendChat(InputField input)
    {
        if (input.text != "")
        {
            NetworkManager.instance.SendChat(PlayerDataManager.instance.my.name, input.text, 1);

            input.text = "";
        }
    }

    IEnumerator ScrollChatLog(Scrollbar bar, float value)
    {
        while (bar.value != value)
        {
            bar.value = Mathf.Lerp(bar.value, value, Time.deltaTime * 10f);
            yield return null;
        }
        scrollChat = null;
    }

    Coroutine scrollChat = null;

    public void AddChatLog(string message)
    {
        chatLog.text += message + "\n";
        if (scrollChat != null)
            StopCoroutine(scrollChat);

        scrollChat = StartCoroutine(ScrollChatLog(chatLog.transform.parent.parent.GetChild(1).GetComponent<Scrollbar>(), 0));
    }


    public void OnExitRoom()
    {
        SoundManager.instance.PlayButtonBGS();
        if (PlayerDataManager.instance.my.isHost)
            NetworkManager.instance.SendReHost();

        NetworkManager.instance.SendExit();
    }

    public void OnReadyOrStart()
    {
        SoundManager.instance.PlayButtonBGS();
        if (PlayerDataManager.instance.my.isHost)
        {
            if (NetworkManager.instance.enterRoom.readyPlayers >= 2 && NetworkManager.instance.enterRoom.readyPlayers == NetworkManager.instance.enterRoom.countPlayers)
                NetworkManager.instance.SendStart();
        }
        else
        {
            if (isReady)
                return;

            isReady = !(PlayerDataManager.instance.my.isReady);
            NetworkManager.instance.SendReady(!(PlayerDataManager.instance.my.isReady));
        }
    }




}
