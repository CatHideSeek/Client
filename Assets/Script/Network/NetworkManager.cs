using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

/// <summary>
/// 서버와 통신하는 유일한 클래스입니다.
/// </summary>
public class NetworkManager : MonoBehaviour
{
    /// <summary>
    /// 싱글톤
    /// </summary>
    public static NetworkManager instance;

    /// <summary>
    /// socket io 
    /// </summary>
    private SocketIOComponent socket;

    /// <summary>
    /// 방 리스트
    /// </summary>
    public List<Room> roomList;

    /// <summary>
    /// 유일한 클라이언트 정보
    /// </summary>
    PlayerDataManager playerData;

    /// <summary>
    /// 입장한 방 정보
    /// </summary>
    [SerializeField]
    public Room enterRoom = new Room();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            socket = GetComponent<SocketIOComponent>();
        }
        else
            Destroy(this.gameObject);

    }

    //Socket 이벤트를 받으러면 On함수들을 Start 에서 선언해야 됩니다.
    void Start()
    {
        playerData = PlayerDataManager.instance;

        #region DB
        socket.On("login", null);
        socket.On("register", null);
        #endregion

        #region ReceiveRoomData
        socket.On("roomList", OnRoomList);
        socket.On("userList", OnUserList);

        socket.On("lobbyCreate", OnRoomCreate);
        socket.On("lobbyJoin", OnRoomJoin);
        socket.On("lobbyStart", OnRoomStart);
        socket.On("lobbyDelet", OnRoomDelet);


        socket.On("roomEnter", OnEnter);
        socket.On("roomJoin", OnJoin);
        socket.On("roomReady", OnReady);
        socket.On("roomStart", OnStart);
        socket.On("roomExit", OnExit);
        socket.On("roomOut", OnOut);
        #endregion

        #region ReceivePlayerData
        socket.On("move", OnPosition);
        socket.On("rotate", OnRotation);
        socket.On("state", OnState);
        socket.On("getKeyPart", OnGetKeyPart);
        socket.On("getKey", OnGetKey);
        socket.On("rootTag", OnRootTag);
        socket.On("childTag", OnChildTag);
        #endregion

        #region ReceiveInGameData
        socket.On("portalCreate", OnPortal);
        socket.On("portalOpen", OnOpen);
        socket.On("portalClose", OnClose);
        #endregion

        #region etc
        socket.On("chat", OnChat);
        #endregion

        StartCoroutine("test");
    }

    IEnumerator test()
    {
        yield return new WaitForSeconds(0.5f);
        SendEnter(enterRoom.id + enterRoom.name);
        yield return new WaitForSeconds(0.5f);
        SendJoin();
    }

    #region RoomMethod

    public void OnRoomList(SocketIOEvent e)
    {
        LitJson.JsonData json = LitJson.JsonMapper.ToObject(e.data.ToString());

        for (int i = 0; i < json["roomList"].Count; ++i)
        {
            Room room = new Room();

            int.TryParse(json["roomList"][i]["id"].ToString(), out room.id);
            room.name = json["roomList"][i]["name"].ToString();
            int.TryParse(json["roomList"][i]["countPlayers"].ToString(), out room.countPlayers);
            int.TryParse(json["roomList"][i]["readyPlayers"].ToString(), out room.readyPlayers);
            int.TryParse(json["roomList"][i]["maxPlayers"].ToString(), out room.maxPlayers);
            bool.TryParse(json["roomList"][i]["isPlayed"].ToString(), out room.isPlay);

            roomList.Add(room);
        }

    }

    public void SendRoomList()
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("a", 0);

        socket.Emit("roomList", json);

    }

    public void OnRoomCreate(SocketIOEvent e)
    {
        JSONObject json = e.data;

        Room room = new Room();

        int.TryParse(json["room"]["id"].ToString(), out room.id);
        room.name = json["room"]["name"].ToString();
        int.TryParse(json["room"]["countPlayers"].ToString(), out room.countPlayers);
        int.TryParse(json["room"]["readyPlayers"].ToString(), out room.readyPlayers);
        int.TryParse(json["room"]["maxPlayers"].ToString(), out room.maxPlayers);
        bool.TryParse(json["room"]["isPlayed"].ToString(), out room.isPlay);

        roomList.Add(room);
    }

    public void OnRoomJoin(SocketIOEvent e)
    {
        JSONObject json = e.data;
        

        string name = json.GetField("roomName").str;
        Room r = FindRoom(name);
        if (r != null)
            int.TryParse(json.GetField("roomName").str, out r.countPlayers);
    }

    public void OnRoomStart(SocketIOEvent e)
    {
        JSONObject json = e.data;
        string name = json.GetField("roomName").str;
        Room r = FindRoom(name);
        if (r != null)
            bool.TryParse(json.GetField("isPlay").str, out r.isPlay);
    }

    public void OnRoomDelet(SocketIOEvent e)
    {
        JSONObject json = e.data;
        string name = json.GetField("roomName").str;
        DeletRoom(name);
    }

    public void SendCreate(string name, int maxPlayers)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("roomName", name);
        json.AddField("maxPlayers", maxPlayers);

        socket.Emit("roomCreate", json);
    }

    public void OnEnter(SocketIOEvent e)
    {
        JSONObject json = e.data;
        string name = json.GetField("roomName").str;

        bool.TryParse(json.GetField("isHost").str, out playerData.my.isHost);

        //입장한 방 세팅
        //enterRoom = FindRoom(name);
        //씬 이동
        //씬 ~ 이 ~ 동 ~ 코 ~ 드 ~

    }

    public void SendEnter(string name)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("roomName", name);
        socket.Emit("roomEnter", json);
    }

    public void OnJoin(SocketIOEvent e)
    {

        JSONObject json = e.data;
        

        string name = json.GetField("name").str;
        string socketID = json.GetField("socketID").str;
        
        User user = new User(name, socketID);

        enterRoom.userList.Add(user);
        

        if (name == playerData.my.name)
        {
            user.isPlayer = true;
            playerData.my.name = name;
            playerData.my.socketID = socketID;
        }
        

        GameObject g = Instantiate(GameManager.instance.playerObject);
        g.GetComponent<PlayerController>().SetUser(user);
        g.name = user.name;
        
    }

    public void SendJoin()
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        playerData.my.name = "User"+Random.Range(1, 100);

        json.AddField("name", playerData.my.name);
        json.AddField("isHost", playerData.my.isHost);
        json.AddField("characterKind", playerData.my.characterKind);

        socket.Emit("roomJoin", json);
    }

    public void OnUserList(SocketIOEvent e)
    {

        LitJson.JsonData json = LitJson.JsonMapper.ToObject(e.data.ToString());
        

        for (int i = 0; i < json["userList"].Count; ++i)
        {
            User user = new User();

            user.socketID = json["userList"][i]["socketID"].ToString();
            user.name = json["userList"][i]["name"].ToString();
            

            if (playerData.my.name != user.name)
            {
                enterRoom.userList.Add(user);
                GameObject g = Instantiate(GameManager.instance.playerObject);
                g.GetComponent<PlayerController>().SetUser(user);
                g.name = user.name;
            }
        }
    }

    public void OnReady(SocketIOEvent e)
    {
        JSONObject json = e.data;

        print(json);
        
        print("ready");
        User user = enterRoom.FindUserBySocketID(json.GetField("socketID").str);
        if (user != null)
        {
            print(user.name);
            user.isReady = json.GetField("isReady").b;
            enterRoom.readyPlayers = (int)json.GetField("readyPlayers").f;
        }
        else
            print("user is null");
    }

    public void SendReady(bool isReady)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("isReady", isReady);

        socket.Emit("roomReady", json);

    }

    public void OnStart(SocketIOEvent e)
    {
        JSONObject json = e.data;

        //게임 시작 함수~
        GameManager.instance.isPlay = true;

        print("game is play");
    }

    public void SendStart()
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("a", 0);

        socket.Emit("roomStart", json);

    }


    public void OnExit(SocketIOEvent e)
    {
        JSONObject json = e.data;
        enterRoom.DeletUser(json.GetField("name").str);
    }

    public void OnOut(SocketIOEvent e)
    {
        JSONObject json = e.data;

        //로비로 나가요.
        //씬 ~ 이 ~ 동 ~ 코 ~ 드 ~
    }

    public void SendExit()
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("a", 0);

        socket.Emit("roomExit", json);

    }
    #endregion

    #region PlayerControllMethod

    /// <summary>
    /// 서버에서 받은 좌표를 세팅합니다.
    /// </summary>
    /// <param name="e"></param>
    public void OnPosition(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("name").str;
        float x = json.GetField("x").f;
        float y = json.GetField("y").f;
        float z = json.GetField("z").f;

        User u = enterRoom.FindUserByName(name);
       

        if (u != null)
        {
            u.controller.SetPosition(new Vector3(x, y, z));
        }

    }

    /// <summary>
    /// 움직인 좌표를 다른 클라이언트에게 보냅니다.
    /// </summary>
    /// <param name="pos">위치 Vector3</param>
    public void SendPosition(Vector3 pos)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("name", playerData.my.name);
        json.AddField("x", pos.x);
        json.AddField("y", pos.y);
        json.AddField("z", pos.z);

        socket.Emit("move", json);
    }

    /// <summary>
    /// 서버에서 받은 좌표를 세팅합니다.
    /// </summary>
    /// <param name="e"></param>
    public void OnRotation(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("name").str;
        float x = json.GetField("x").f;
        float y = json.GetField("y").f;
        float z = json.GetField("z").f;
        float w = json.GetField("w").f;

        User u = enterRoom.FindUserByName(name);
        

        if (u != null)
        {
            u.controller.SetRotation(new Quaternion(x, y, z, w));
        }

    }

    /// <summary>
    /// 움직인 좌표를 다른 클라이언트에게 보냅니다.
    /// </summary>
    /// <param name="pos">위치 Vector3</param>
    public void SendRotation(Quaternion rot)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("name", playerData.my.name);
        json.AddField("x", rot.x);
        json.AddField("y", rot.y);
        json.AddField("z", rot.z);
        json.AddField("w", rot.w);

        socket.Emit("rotate", json);
    }

    public void OnState(SocketIOEvent e)
    {
        JSONObject json = e.data;
        int state = 0;
        int.TryParse(json.GetField("state").str, out state);

        string name = json.GetField("name").str;
        User u = enterRoom.FindUserByName(name);

        if (state < 0)
        {
            int index = u.FindState(Mathf.Abs(state));
            u.states.RemoveAt(index);
        }
        else if (state > 0)
        {
            u.states.Add((User.State)state);
        }
        else
        {
            u.states.Clear();
            u.states.Add(User.State.Normal);
        }
    }

    public void SendState(int state)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("name", playerData.my.name);
        json.AddField("state", state);

        socket.Emit("state", json);
    }

    public void OnGetKeyPart(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("name").str;

        User u = enterRoom.FindUserByName(name);

        if (u != null)
        {
            int.TryParse(json.GetField("part").str, out u.keyCount);
        }
    }

    public void SendGetKeyPart(int count)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("name", playerData.my.name);
        json.AddField("part", count);

        socket.Emit("getKeyPart", json);
    }

    public void OnGetKey(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("name").str;

        User u = enterRoom.FindUserByName(name);

        if (u != null)
        {
            bool.TryParse(json.GetField("have").str, out u.isKeyHave);
        }
    }

    public void SendGetKey(bool have)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("name", playerData.my.name);
        json.AddField("have", have);

        socket.Emit("getKey", json);
    }

    public void OnRootTag(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("tager").str;

        User u = enterRoom.FindUserByName(name);

        if (u != null)
        {
            u.isBoss = true;
        }

    }

    public void SendRootTag(string name)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("tager", name);

        socket.Emit("rootTag", json);

    }

    public void OnChildTag(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("child").str;

        User u = enterRoom.FindUserByName(name);

        if (u != null)
        {
            u.isBossChild = true;
        }

    }

    public void SendChildTag(string name, string other)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("tager", name);
        json.AddField("child", other);

        socket.Emit("childTag", json);

    }

    public void OnPortal(SocketIOEvent e)
    {
        JSONObject json = e.data;
        float x = json.GetField("x").f;
        float y = json.GetField("y").f;
        float z = json.GetField("z").f;
        Vector3 pos = new Vector3(x, y, z);
        //포탈 오브젝트 생성

    }

    public void SendPortal(Vector3 pos)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("x", pos.x);
        json.AddField("y", pos.y);
        json.AddField("z", pos.z);

        socket.Emit("portalCreate", json);
    }

    public void OnOpen(SocketIOEvent e)
    {
        JSONObject json = e.data;
        string name = json.GetField("name").str;
        //포탈 오픈!

    }

    public void SendOpen(string name)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("name", name);

        socket.Emit("portalOpen", json);
    }

    public void OnClose(SocketIOEvent e)
    {
        JSONObject json = e.data;
        //포탈 닫힘!

    }

    public void SendClose()
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("a", 0);

        socket.Emit("portalClose", json);

    }

    public void OnChat(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("name").str;
        string message = json.GetField("message").str;
        int where = 0;
        int.TryParse(json.GetField("where").str, out where);
    }

    public void SendChat(string name, string message, int where)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("name", name);
        json.AddField("message", message);
        json.AddField("where", where);

        socket.Emit("chat", json);
    }



    #endregion


    #region util

    Room FindRoom(string name)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].id + roomList[i].name == name)
                return roomList[i];
        }
        return null;
    }

    void DeletRoom(string name)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].id + roomList[i].name == name)
            {
                roomList.RemoveAt(i);
                break;
            }
        }
    }
    #endregion

}
