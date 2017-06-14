using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 로비 UI 클래스입니다.
/// </summary>
public class UILobby : MonoBehaviour
{

    public static UILobby instance;

    public Animator menuAni, rommListAni, roomListChatLogAni, roomListChatInputAni;

    public Text roomListText;

    public GameObject roomSlot;
    public List<GameObject> roomSlotList;

    public Vector2 startPos, marginPos;

    public RectTransform roomScrollView;

    bool isOpenRoomList = false, isOpenMenu = false;

    public Text[] chatLog;

    private void Awake()
    {
        instance = this;
    }

    void CreateRoomList()
    {
        //NetworkManager.instance.roomList;
        //roomScrollView

        roomScrollView.sizeDelta = new Vector2(0, NetworkManager.instance.roomList.Count * -marginPos.y + 10f);

        for (int i = 0; i < NetworkManager.instance.roomList.Count; i++)
        {
            GameObject g = Instantiate(roomSlot);
            roomSlotList.Add(g);
            g.GetComponent<UIRoomSlot>().SetData(i + 1, NetworkManager.instance.roomList[i]);
            RectTransform tr = g.GetComponent<RectTransform>();
            tr.parent = roomScrollView;
            tr.localPosition = startPos + new Vector2(marginPos.x, marginPos.y * i);
            tr.localScale = new Vector2(1, 1);
        }

    }

    void RemoveRoomList()
    {
        if (roomSlotList.Count < 1)
            return;

        for (int i = 0; i < roomSlotList.Count; i++)
        {
            Destroy(roomSlotList[i]);
        }
    }

    public void OnMenu()
    {
        isOpenMenu = !isOpenMenu;
        if (isOpenMenu)
            menuAni.SetTrigger("Open");
        else
            menuAni.SetTrigger("Close");
    }

    public void OnroomList()
    {
        isOpenRoomList = !isOpenRoomList;
        if (isOpenRoomList)
        {
            RemoveRoomList();
            rommListAni.SetTrigger("Open");
            roomListChatInputAni.SetTrigger("Open");
            roomListChatLogAni.SetTrigger("Open");
            CreateRoomList();
            roomListText.text = "Close";
        }
        else
        {
            rommListAni.SetTrigger("Close");
            roomListChatInputAni.SetTrigger("Close");
            roomListChatLogAni.SetTrigger("Close");
            roomListText.text = "Open";
        }
    }

    public void SendChat(InputField input)
    {
        //NetworkManager.instance.SendChat(PlayerDataManager.instance.my.name, input.text, 0);
        for (int i = 0; i < chatLog.Length; i++)
        {
            chatLog[i].text += "User : " + input.text + "\n";
            chatLog[i].transform.parent.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
        }

        input.text = "";
    }

    public void AddChatLog(string message)
    {
        for (int i = 0; i < chatLog.Length; i++)
        {
            chatLog[i].text += message + "\n";
        }
    }


}
