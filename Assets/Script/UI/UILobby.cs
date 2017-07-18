using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 로비 UI 클래스입니다.
/// </summary>
public class UILobby : MonoBehaviour
{
    
    public Animator rommListAni, ChatLogAni, ChatInputAni;

    public Text roomListText;

    public GameObject roomSlot;
    public List<GameObject> roomSlotList;

    public Vector2 startPos, marginPos;

    public RectTransform roomScrollView;

    bool isOpenRoomList = false;

    public Text chatLog;

    public GameObject pwInput, wrongPw;


    public void CreateRoomList()
    {

        roomScrollView.sizeDelta = new Vector2(0, NetworkManager.instance.roomList.Count * -marginPos.y + 10f);

        for (int i = 2; i < NetworkManager.instance.roomList.Count; i++)
        {

            if (NetworkManager.instance.roomList[i].id != 0)
            {
                GameObject g = Instantiate(roomSlot);
                roomSlotList.Add(g);
                g.GetComponent<UIRoomSlot>().SetData((i-2) + 1, NetworkManager.instance.roomList[i]);
                RectTransform tr = g.GetComponent<RectTransform>();
                tr.SetParent(roomScrollView);
                tr.localPosition = startPos + new Vector2(marginPos.x, marginPos.y * (i - 2));
                tr.localScale = new Vector2(1, 1);
            }
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

    public void OnroomList()
    {
        isOpenRoomList = !isOpenRoomList;
        if (isOpenRoomList)
        {
            RemoveRoomList();
            rommListAni.SetTrigger("Open");
            ChatInputAni.SetTrigger("Open");
            ChatLogAni.SetTrigger("Open");
            roomListText.text = "닫기";
        }
        else
        {
            rommListAni.SetTrigger("Close");
            ChatInputAni.SetTrigger("Close");
            ChatLogAni.SetTrigger("Close");
            roomListText.text = "방 리스트";
        }

        NetworkManager.instance.SendRoomList();
    }

    public void SendChat(InputField input)
    {
        if (input.text != "")
        {
            NetworkManager.instance.SendChat(PlayerDataManager.instance.my.name, input.text, 0);

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

    public void SetPwInput(Room room) {
        pwInput.SetActive(true);
        pwInput.GetComponent<UIPWInput>().SetRoomData(room);
    }

    public void SetWrongPw() {
        wrongPw.SetActive(true);
    }

}
