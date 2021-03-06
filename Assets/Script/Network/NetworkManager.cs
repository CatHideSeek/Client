﻿using System.Collections;
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

    public GameObject playerObject;
    public GameObject playerInfoUI;

    public bool isMapLoaded = false;

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

        #region Error
        socket.On("errorcode", OnErrorCode);
        #endregion

        #region DB
        socket.On("register", OnRegister);
        #endregion

        #region ReceiveRoomData
        socket.On("roomList", OnRoomList);
        socket.On("userList", OnUserList);

        socket.On("lobbyEnter", OnLobbyEnter);
        socket.On("lobbyEnterUser", OnLobbyEnterUser);
        socket.On("lobbyCreate", OnRoomCreate);
        socket.On("lobbyJoin", OnRoomJoin);
        socket.On("lobbyStart", OnRoomStart);
        socket.On("lobbyDelet", OnRoomDelet);

        socket.On("notice", OnNoticeData);

        socket.On("waitRoomEnter", OnWaitRoomEnter);
        socket.On("waitRoomJoin", OnWaitRoomJoin);
        socket.On("reHost", OnReHost);

        socket.On("roomReady", OnReady);
        socket.On("roomStart", OnStart);

        socket.On("roomEnter", OnEnter);
        socket.On("roomJoin", OnJoin);
        socket.On("roomLoad", OnLoading);
        socket.On("roomPlay", OnPlay);
        socket.On("roomExit", OnExit);
        socket.On("roomOut", OnOut);

        #endregion

        #region ReceivePlayerData
        socket.On("modelType", OnModelType);
        socket.On("move", OnPosition);
        socket.On("rotate", OnRotation);
        socket.On("state", OnState);
        socket.On("getKeyPart", OnGetKeyPart);
        socket.On("getKey", OnGetKey);
        socket.On("rootTag", OnRootTag);
        socket.On("childTag", OnChildTag);
        #endregion

        #region ReceiveInGameData
        socket.On("initEnd", OnInitEnd);
        socket.On("trap", OnTrap);
        socket.On("generator", OnGenerator);
        socket.On("spawnPos", OnSpawnPos);
        socket.On("map", OnBlock);
        socket.On("remap", OnReMap);
        socket.On("portalCreate", OnPortal);
        socket.On("portalOpen", OnOpen);
        socket.On("portalClose", OnClose);
        socket.On("escape", OnEscape);
        #endregion

        #region etc
        socket.On("chat", OnChat);
        #endregion


    }

    IEnumerator EnterTheRoom()
    {
        yield return new WaitForSeconds(0.5f);
        SendEnter(enterRoom.id + enterRoom.name);
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.mapGenerator.InitMap();
        SendJoin();
    }


    #region ErrorMethod

    public void OnErrorCode(SocketIOEvent e)
    {
        JSONObject json = e.data;
        int code = (int)(json.GetField("code").f);

        switch (code)
        {
            case 0: GameObject.FindWithTag("UIManager").GetComponent<UITitle>().nameError.SetActive(true); break;
        }

    }

    #endregion

    #region DataMethod

    public void OnRegister(SocketIOEvent e)
    {

        JSONObject json = e.data;
        string socketID = json.GetField("socketID").str;
        string name = json.GetField("name").str;

        playerData.my.socketID = socketID;
        playerData.my.name = name;
        SceneLoadManager.instance.LoadScene(SceneLoadManager.instance.OnLobby);

    }

    public void SendRegister(string name)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("name", name);

        socket.Emit("register", json);
    }

    #endregion


    #region RoomMethod

    public void OnLobbyEnter(SocketIOEvent e)
    {

        LitJson.JsonData json = LitJson.JsonMapper.ToObject(e.data.ToString());

        enterRoom = FindRoom("0Lobby");

        enterRoom.userList.Clear();

        for (int i = 0; i < json["userList"].Count; i++)
        {
            //print(json["userList"][i]["socketID"].ToString() + "\n" + json["userList"][i]["name"].ToString());

            string socketID = json["userList"][i]["socketID"].ToString();
            string name = json["userList"][i]["name"].ToString();


            if (playerData.my.name != name)
            {

                User user = new User(name, socketID);

                enterRoom.AddUser(user);

                GameObject g = Instantiate(playerObject, Vector3.zero + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)), Quaternion.identity);
                g.GetComponent<PlayerController>().SetUser(user);

                g.name = user.name;

            }
        }

    }

    public void OnLobbyEnterUser(SocketIOEvent e)
    {
        JSONObject json = e.data;

        //print("join is work");

        string name = json.GetField("name").str;
        string socketID = json.GetField("socketID").str;


        User user = new User(name, socketID);

        enterRoom.AddUser(user);


        GameObject g = Instantiate(playerObject, Vector3.zero + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)), Quaternion.identity);
        if (name == playerData.my.name)
        {
            playerData.my = user;
            user.isPlayer = true;
        }
        g.GetComponent<PlayerController>().SetUser(user);
        g.name = user.name;

        if (name == playerData.my.name)
        {
            int r = Random.Range(0, 5);
            playerData.SetCatModel(r);//고양이 종류를 정해줍니다.

        }
        SendPosition(playerData.my.controller.transform.position, Vector3.zero, 0, 0, true);//방금 입장한 사람을 위해 현재좌표를 전송
        SendModelType(playerData.modelType);//방금 입장한 사람을 위해 내 모델타입을 전송

    }

    public void SendLobbyEnter()
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("name", playerData.my.name);

        socket.Emit("lobbyEnter", json);
    }

    public void OnNoticeData(SocketIOEvent e)
    {
        JSONObject json = e.data;

        print("set notice");

        string name = json.GetField("userName").str;
        string context = json.GetField("context").str;

        GameObject.FindGameObjectWithTag("UIManager").GetComponent<UILobby>().SetNotice(context, name);
    }

    public void SendNoticeData(string context, string userName)
    {

        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("context", context);
        json.AddField("userName", userName);

        socket.Emit("notice", json);
    }

    public void OnRoomList(SocketIOEvent e)
    {
        roomList.Clear();

        LitJson.JsonData json = LitJson.JsonMapper.ToObject(e.data.ToString());

        for (int i = 0; i < json["roomList"].Count; ++i)
        {
            Room room = new Room();

            int.TryParse(json["roomList"][i]["id"].ToString(), out room.id);
            room.name = json["roomList"][i]["name"].ToString().Replace("\"", "");
            int.TryParse(json["roomList"][i]["countPlayers"].ToString(), out room.countPlayers);
            int.TryParse(json["roomList"][i]["readyPlayers"].ToString(), out room.readyPlayers);
            int.TryParse(json["roomList"][i]["maxPlayers"].ToString(), out room.maxPlayers);
            int.TryParse(json["roomList"][i]["pw"].ToString(), out room.pw);
            bool.TryParse(json["roomList"][i]["isPlay"].ToString(), out room.isPlay);

            roomList.Add(room);
        }

        GameObject.FindGameObjectWithTag("UIManager").GetComponent<UILobby>().CreateRoomList();


    }

    /// <summary>
    /// 서버에게 방 리스트를 요청합니다.
    /// </summary>
    public void SendRoomList()
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("a", 0);

        socket.Emit("roomList", json);

    }

    public void OnWaitRoomEnter(SocketIOEvent e)
    {
        JSONObject json = e.data;
        string name = json.GetField("roomName").str;
        playerData.my.isHost = json.GetField("isHost").b;
        //print(json.GetField("isHost").b);
        enterRoom = FindRoom(name);

        SceneLoadManager.instance.LoadScene(SceneLoadManager.instance.OnWaitRoom);
    }

    public void SendWaitRoomEnter(string name)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("name", playerData.my.name);
        json.AddField("roomName", name);
        socket.Emit("waitRoomEnter", json);
    }

    public void OnWaitRoomJoin(SocketIOEvent e)
    {
        JSONObject json = e.data;

        //print("join is work");

        isMapLoaded = false;

        string name = json.GetField("name").str;
        string socketID = json.GetField("socketID").str;


        User user = new User(name, socketID);
        user.isHost = json.GetField("isHost").b;

        if (user.isHost)
        {
            SendReady(true);
        }

        enterRoom.AddUser(user);
        print(user.name + " is add count at waitroomjoin");
        enterRoom.UpdateCountPlayer(1);


        GameObject g = Instantiate(playerObject, Vector3.zero + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)), Quaternion.identity);
        if (name == playerData.my.name)
        {
            playerData.my = user;
            user.isPlayer = true;
        }
        g.GetComponent<PlayerController>().SetUser(user);
        g.name = user.name;

        if (name == playerData.my.name)
        {
            int r = Random.Range(0, 5);
            playerData.SetCatModel(r);//고양이 종류를 정해줍니다.
        }

        SendPosition(playerData.my.controller.transform.position, Vector3.zero, 0, 0, true);//방금 입장한 사람을 위해 현재좌표를 전송
        SendModelType(playerData.modelType);//방금 입장한 사람을 위해 내 모델타입을 전송
        enterRoom.UpdateUI();
        GameObject.FindWithTag("UIManager").GetComponent<UIWaitRoom>().SetRoomInfo(enterRoom);
    }

    public void SendWaitRoomJoin()
    {
        GameObject.FindWithTag("UIManager").GetComponent<UIWaitRoom>().SetRoomInfo(enterRoom);

        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("name", playerData.my.name);
        json.AddField("isHost", playerData.my.isHost);

        socket.Emit("waitRoomJoin", json);
    }

    public void OnRoomJoin(SocketIOEvent e)
    {
        JSONObject json = e.data;


        string name = json.GetField("roomName").str;
        Room r = FindRoom(name);
        if (r != null)
        {
            print(json.GetField("count").str);
            r.countPlayers = (int)(json.GetField("count").f);
            print("asd " + r.countPlayers);
            r.UpdateUI();
        }
    }

    public void OnRoomStart(SocketIOEvent e)
    {
        JSONObject json = e.data;
        string name = json.GetField("roomName").str;
        Room r = FindRoom(name);
        if (r != null)
        {
            bool.TryParse(json.GetField("isPlay").str, out r.isPlay);
            r.UpdateUI();
        }
    }

    public void OnRoomDelet(SocketIOEvent e)
    {
        JSONObject json = e.data;
        string name = json.GetField("roomName").str;
        print("room delet" + name);
        DeletRoom(name);
    }

    public void OnRoomCreate(SocketIOEvent e)
    {
        JSONObject json = e.data;


        //print("room Create");

        Room room = new Room();

        int.TryParse(json["room"]["id"].ToString(), out room.id);
        room.name = json["room"]["name"].ToString().Replace("\"", "");
        int.TryParse(json["room"]["countPlayers"].ToString(), out room.countPlayers);
        int.TryParse(json["room"]["readyPlayers"].ToString(), out room.readyPlayers);
        int.TryParse(json["room"]["maxPlayers"].ToString(), out room.maxPlayers);
        int.TryParse(json["room"]["pw"].ToString(), out room.pw);
        bool.TryParse(json["room"]["isPlay"].ToString(), out room.isPlay);

        roomList.Add(room);

        if (bool.Parse(json["host"].ToString()) == false)
            GameObject.FindGameObjectWithTag("UIManager").GetComponent<UILobby>().CreateRoomList();
    }

    /// <summary>
    /// 서버로 방 생성을 전송합니다.
    /// </summary>
    /// <param name="name">방 이름</param>
    /// <param name="maxPlayers">최대 입장 가능 수</param>
    public void SendRoomCreate(string name, int maxPlayers, int pw)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("roomName", name);
        json.AddField("maxPlayers", maxPlayers);
        json.AddField("pw", pw);

        socket.Emit("roomCreate", json);
        //자동으로 방으로 입장 됨.
    }

    public void OnEnter(SocketIOEvent e)
    {
        JSONObject json = e.data;

        //Debug.Log ("로딩 전");
        enterRoom.userList.Clear();
        SceneLoadManager.instance.LoadScene(SceneLoadManager.instance.OnInGame);
        //Debug.Log ("로딩 후");
    }

    /// <summary>
    /// 서버로 인게임 입장 시도를 보냅니다.
    /// </summary>
    /// <param name="name">방 이름 = index + name </param>
    public void SendEnter(string name)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("roomName", name);
        socket.Emit("roomEnter", json);
    }

    public void OnLoading(SocketIOEvent e)
    {
        Debug.Log("OnLoading()");
        JSONObject json = e.data;


        enterRoom.loadingPlayers = (int)(json.GetField("load").f);

        print(enterRoom.loadingPlayers + " on loading");

        if (playerData.my.isHost && enterRoom.loadingPlayers == enterRoom.countPlayers)
        {
            print("SendMap()");
            //SendPlay();
            // 생성된 맵 데이터를 모두에게 전송
            enterRoom.loadingPlayers = 0;
            print(enterRoom.loadingPlayers + " is send map");
            SendMap();
        }
    }

    public void SendLoading()
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        socket.Emit("loading", json);
    }


    public void OnJoin(SocketIOEvent e)
    {
        //Debug.Log ("OnJoin()");
        JSONObject json = e.data;

        //print("Join Is work");

        string name = json.GetField("name").str;
        string socketID = json.GetField("socketID").str;
        bool isHost = json.GetField("isHost").b;

        print("Join : " + name);

        User user = new User(name, socketID);
        user.isHost = isHost;

        enterRoom.AddUser(user);

        if (name == playerData.my.name)
        {
            playerData.my = user;
            user.isPlayer = true;
        }

        GameObject g = Instantiate(GameManager.instance.playerObject, GameManager.instance.spawnPos, Quaternion.identity);
        g.GetComponent<PlayerController>().SetUser(user);
        g.name = user.name;
        g.GetComponent<Rigidbody>().useGravity = false;

        if (user.isPlayer)
        {
            int r = Random.Range(0, 5);
            playerData.SetCatModel(r);//고양이 종류를 정해줍니다.
            if (r == 3)
                user.controller.arrow.SetActive(true);
            GameManager.instance.isModelReady = true;

            SendLoading();
        }


        //로딩이 완료됨 을 알림
        SendPosition(playerData.my.controller.transform.position, Vector3.zero, 0, 0, true);
        SendModelType(playerData.modelType);

    }

    /// <summary>
    /// 서버로 방 입장에 성공했음을 보냅니다.
    /// </summary>
    public void SendJoin() // ㄱㄷㄱㄷ
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        //playerData.my.name = "User" + Random.Range(1, 100);

        json.AddField("name", playerData.my.name);
        json.AddField("isHost", playerData.my.isHost);
        json.AddField("characterKind", playerData.my.characterKind);

        socket.Emit("roomJoin", json);
    }

    public void OnBlock(SocketIOEvent e)
    {
        JSONObject json = e.data;

        //print("OnbLOCK");


        float x = json.GetField("bx").f;
        float y = json.GetField("by").f;
        float z = json.GetField("bz").f;
        int id = (int)json.GetField("bid").f;
        int parent = (int)json.GetField("parent").f;
        bool isFinal = json.GetField("isFinal").b;

        print("is final " + isFinal);

        GameObject block;
        if (id == 13)
            GameManager.instance.spawnList.Add(new Vector3(x, y, z));
        if (id == 12)
        {
            block = Instantiate(GameManager.instance.blockObject[id], new Vector3(x, y, z), Quaternion.Euler(-90, 0, 0));
            GameManager.instance.portalObject = block;
            GameManager.instance.portal = block.GetComponent<PortalController>();
        }
        else
        {
            block = Instantiate(GameManager.instance.blockObject[id], new Vector3(x, y, z), GameManager.instance.blockObject[id].transform.rotation);
        }

        if (id == 6 || id == 7)
            block.transform.parent = MapGenerator.instance.transform;
        else
        {
            Transform parentIsland = GameObject.Find("Island " + parent).transform;
            block.transform.parent = parentIsland;
            if (id >= 0 && id <= 3)
                parentIsland.GetComponent<IslandGenerator>().blocks.Add(block);

            if (id == 11)
                block.GetComponent<Spawner>().s = parent % 3;
        }

        if (isFinal)
        {
            MapGenerator.instance.transform.Rotate(new Vector3(0, 45, 0));
            SendInitEnd();
        }

        //오브젝트를 받아와서 배치함(id값은 0~3:땅, 4: 나무, 5: 수풀, 6~7: 다리, 8:키스포너)

    }


    /// <summary>
    /// 블럭의 좌표를 다른 클라이언트에게 보냅니다.
    /// </summary>
    /// <param name="pos">위치 Vector3</param>
    /// <param name="id">블럭 종류</param>
    /// <param name="parent">부모</param>
    public void SendBlock(Vector3 pos, int id, int parent, bool isFinal)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("bx", pos.x);
        json.AddField("by", pos.y);
        json.AddField("bz", pos.z);
        json.AddField("bid", id);
        json.AddField("parent", parent);
        json.AddField("isFinal", isFinal);

        socket.Emit("map", json);
    }

    public void OnEscape(SocketIOEvent e)
    {
        JSONObject json = e.data;
        string name = json.GetField("name").str;

        UIInGame.instance.ViewNotice(name + "(이)가 탈출에 성공하였습니다");
        enterRoom.FindUserByName(name).controller.clear = true;
        GameManager.instance.isEscape = true;
    }

    public void SendEscape(string name)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("name", name);

        socket.Emit("escape", json);
    }

    public void OnReMap(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("name").str;

    }

    //heresend

    public void SendMap()
    {
        if (PlayerDataManager.instance.my.isHost)
        {
            Debug.Log("SendMap()");

            SendSpawnPos(GameManager.instance.spawnPos.y);
            foreach (IslandInfo info in GameManager.instance.islandList)
            {
                SendGenerator(info.x, info.y, info.id);//생성기 먼저 생성해야됨
            }
            //바꿔야됨
            for (int i = 0; i < GameManager.instance.blockList.Count; i++)
            {
                SendBlock(GameManager.instance.blockList[i].pos, GameManager.instance.blockList[i].id, GameManager.instance.blockList[i].parent, false);
                if (i == GameManager.instance.blockList.Count - 1)
                    SendBlock(GameManager.instance.blockList[i].pos, GameManager.instance.blockList[i].id, GameManager.instance.blockList[i].parent, true);
            }

            SendInitEnd();
        }

    }

    public void OnInitEnd(SocketIOEvent e)
    {
        JSONObject json = e.data;

        print("initEnd");

        if (PlayerDataManager.instance.my.isHost)
        {
            enterRoom.loadingPlayers++;

            print(enterRoom.loadingPlayers + "in initend");

            if (enterRoom.loadingPlayers == enterRoom.countPlayers)
            {
                SendPlay();
            }
        }

    }

    /// <summary>
    /// 맵생성이 끝낫음을 알려준다
    /// </summary>
    public void SendInitEnd()
    {
        print("SendInitEnd()");
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("target", 1);
        socket.Emit("initEnd", json);
    }

    public void OnGenerator(SocketIOEvent e)
    {
        JSONObject json = e.data;

        //print("OnGenerator ");

        int x = (int)json.GetField("x").f;
        int y = (int)json.GetField("y").f;
        int id = (int)json.GetField("id").f;

        MapGenerator.instance.CreateIsland(x, y, id);
    }

    /// <summary>
    /// 섬생성기 정보를 다른 클라이언트에게 보냅니다.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="id"></param>
    public void SendGenerator(int x, int y, int id)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("x", x);
        json.AddField("y", y);
        json.AddField("id", id);

        socket.Emit("generator", json);
    }

    public void OnTrap(SocketIOEvent e)
    {
        JSONObject json = e.data;

        float x = json.GetField("x").f;
        float y = json.GetField("y").f;
        float z = json.GetField("z").f;
        string owner = json.GetField("owner").str;
        bool stun = json.GetField("stun").b;

        if (owner.Equals(PlayerDataManager.instance.my.name))
            return;

        Trap t = Instantiate(GameManager.instance.blockObject[9], new Vector3(x, y, z), GameManager.instance.blockObject[9].transform.rotation).GetComponent<Trap>();
        t.SetOwner(owner);
        t.stun = stun;
    }

    /// <summary>
    /// 트랩생성 정보를 다른 클라이언트에게 보냅니다.
    /// </summary>
    /// <param name="pos"></param>
    public void SendTrap(Vector3 pos, bool stun)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("x", pos.x);
        json.AddField("y", pos.y);
        json.AddField("z", pos.z);
        json.AddField("owner", PlayerDataManager.instance.my.name);
        json.AddField("stun", stun);

        socket.Emit("trap", json);
    }

    public void OnModelType(SocketIOEvent e)
    {
        //Debug.Log("omt");
        JSONObject json = e.data;
        string name = json.GetField("name").str;
        int id = (int)json.GetField("id").f;
        User user = enterRoom.FindUserByName(name);
        //Debug.Log("user cm: "+user.controller.createdModel);

        if (user.controller.createdModel)
            return;

        //Debug.Log("user cm: "+id);


        if (user != null)
            user.controller.SetModel(id);
    }

    /// <summary>
    /// 내 고양이 종류를 다른 클라이언트에게 보냅니다.
    /// </summary>
    /// <param name="id"></param>
    public void SendModelType(int id)
    {
        //Debug.Log("SendModelType()" + id);
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("name", playerData.my.name);
        json.AddField("id", id);

        socket.Emit("modelType", json);
    }

    public void OnSpawnPos(SocketIOEvent e)
    {
        JSONObject json = e.data;

        GameManager.instance.spawnPos.y = json.GetField("sy").f;
        PlayerDataManager.instance.my.controller.transform.position = GameManager.instance.spawnPos + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

    }


    /// <summary>
    /// 스폰좌표를 다른 클라이언트에게 보냅니다.
    /// </summary>
    /// <param name="y">y 좌표</param>
    public void SendSpawnPos(float y)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("sy", y);
        //print("SendSpawnPos");

        socket.Emit("spawnPos", json);
    }

    public void OnUserList(SocketIOEvent e)
    {

        LitJson.JsonData json = LitJson.JsonMapper.ToObject(e.data.ToString());

        enterRoom.UpdateCountPlayer(enterRoom.userList.Count);
        enterRoom.userList.Clear();


        //print("userlist is work");

        for (int i = 0; i < json["userList"].Count; ++i)
        {
            string socketID = json["userList"][i]["socketID"].ToString();
            string name = json["userList"][i]["name"].ToString();
            bool isHost = bool.Parse(json["userList"][i]["isHost"].ToString());
            bool isReady = bool.Parse(json["userList"][i]["isReady"].ToString());

            if (playerData.my.name != name)
            {

                User user = new User(name, socketID);
                user.isHost = isHost;
                user.isReady = isReady;

                enterRoom.AddUser(user);

                GameObject g = Instantiate(playerObject, Vector3.zero + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)), Quaternion.identity);
                g.GetComponent<PlayerController>().SetUser(user);
                g.name = user.name;
            }
        }
    }

    public void OnReady(SocketIOEvent e)
    {
        JSONObject json = e.data;

        //print(json);

        //print("ready");
        User user = enterRoom.FindUserBySocketID(json.GetField("socketID").str);
        if (user != null)
        {
            //print(user.name);
            user.isReady = json.GetField("isReady").b;
            enterRoom.readyPlayers = (int)json.GetField("readyPlayers").f;
            user.controller.infoUI.SetRedayLabel(user.isReady);
            GameObject.FindWithTag("UIManager").GetComponent<UIWaitRoom>().UpdateReadyText(enterRoom);
        }
        //else
        //print("user is null");
    }

    /// <summary>
    /// 서버로 자신의 게임 준비 상태를 보냅니다.
    /// </summary>
    /// <param name="isReady">준비 상태인가?</param>
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
        StartCoroutine("EnterTheRoom");
    }

    /// <summary>
    /// 서버로 게임 시작을 보냅니다.
    /// </summary>
    public void SendStart()
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("a", 0);

        socket.Emit("roomStart", json);
    }

    public void OnPlay(SocketIOEvent e)
    {
        //Debug.Log ("OnPlay");
        //게임 매니저 스타트
        GameManager.instance.StartGame();
    }

    /// <summary>
    /// 모든 유저의 인 게임이 로드되면 플레이가 시작	
    /// </summary>
    public void SendPlay()
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("a", 0);
        socket.Emit("roomPlay", json);
    }

    public void OnExit(SocketIOEvent e)
    {
        JSONObject json = e.data;
        //print("exit work");
        enterRoom.DeletUser(json.GetField("name").str);
    }

    public void OnOut(SocketIOEvent e)
    {
        JSONObject json = e.data;

        //로비로 나가요.
        SceneLoadManager.instance.LoadScene(SceneLoadManager.instance.OnLobby);
    }

    /// <summary>
    /// 서버로 방 나가기를 보냅니다.
    /// </summary>
    public void SendExit()
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("a", 0);

        socket.Emit("roomExit", json);
    }

    public void OnReHost(SocketIOEvent e)
    {
        JSONObject json = e.data;
        User user = enterRoom.FindUserByName(json.GetField("name").str);

        if (user == null)
            return;

        user.isHost = true;

        if (playerData.my.isHost)
            GameObject.FindWithTag("UIManager").GetComponent<UIWaitRoom>().UpdateReadyText(enterRoom);
    }

    public void SendReHost()
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        string name = "";

        if (enterRoom.userList.Count < 2)
            return;

        do
        {
            name = enterRoom.userList[Random.Range(0, enterRoom.userList.Count)].name;
        } while (name == playerData.my.name);

        json.AddField("name", name);

        socket.Emit("reHost", json);

    }

    #endregion

    #region PlayerControllMethod

    public void OnPosition(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("name").str;
        float x = json.GetField("x").f;
        float y = json.GetField("y").f;
        float z = json.GetField("z").f;

        float rx = json.GetField("rx").f;
        float ry = json.GetField("ry").f;
        float rz = json.GetField("rz").f;


        float h = json.GetField("h").f;
        float v = json.GetField("v").f;

        bool isDirect = json.GetField("isDirect").b;

        User user = enterRoom.FindUserByName(name);

        if (user != null)
        {
            user.controller.SetPosition(new Vector3(x, y, z), new Vector3(rx, ry, rz), h, v, isDirect);
        }

    }

    /// <summary>
    /// 움직인 좌표를 다른 클라이언트에게 보냅니다.
    /// </summary>
    /// <param name="pos">위치 Vector3</param>
    public void SendPosition(Vector3 pos, Vector3 vel, float h, float v, bool isDirect)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("name", playerData.my.name);
        json.AddField("x", pos.x);
        json.AddField("y", pos.y);
        json.AddField("z", pos.z);


        json.AddField("rx", vel.x);
        json.AddField("ry", vel.y);
        json.AddField("rz", vel.z);


        json.AddField("h", h);
        json.AddField("v", v);


        json.AddField("isDirect", isDirect);

        socket.Emit("move", json);
    }

    public void OnRotation(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("name").str;
        float x = json.GetField("x").f;
        float y = json.GetField("y").f;
        float z = json.GetField("z").f;
        float w = json.GetField("w").f;

        User user = enterRoom.FindUserByName(name);


        if (user != null)
        {
            user.controller.SetRotation(new Quaternion(x, y, z, w));
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
        int state = (int)json.GetField("state").f;
        int objKind = (int)json.GetField("objKind").f;

        string name = json.GetField("name").str;
        User user = enterRoom.FindUserByName(name);

        if (user == PlayerDataManager.instance.my)
            return;

        if (state > 0)
        {
            if (user.FindState(state) == -1)
            {
                user.PushState((User.State)state);
                if (state == (int)User.State.Change)
                    user.objectKind = objKind;
                //Debug.Log("OnState()" + user.FindState(state));
            }
        }
        else if (state < 0)
        {
            user.PopState((User.State)(-state));
            if (-state == (int)User.State.Change)
                user.objectKind = 0;
        }

    }

    /// <summary>
    /// 유저의 상태를 알립니다.
    /// </summary>
    /// <param name="state">상태 Enum(-는 삭제, +는 추가)</param>
    /// <param name="objKind">변신 오브젝트 종류(0: 기본값)</param>
    public void SendState(int state, int objKind = 0)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("name", playerData.my.name);
        json.AddField("state", state);
        json.AddField("objKind", objKind);

        socket.Emit("state", json);

        //Debug.Log("SendState()");
    }

    public void OnGetKeyPart(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("name").str;

        User user = enterRoom.FindUserByName(name);


        if (user != null)
        {

            user.keyCount = (int)json.GetField("part").f;
            UIInGame.instance.ViewUserState(name + " (이)가 열쇠 조각을 얻었습니다.");
        }
    }

    /// <summary>
    /// 열쇠 조각의 획득을 알립니다.
    /// </summary>
    /// <param name="count">조각 수</param>
    /// <param name="keyPos">획득위치</param>
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

        User user = enterRoom.FindUserByName(name);

        if (user != null)
        {
            user.isKeyHave = json.GetField("have").b;
            UIInGame.instance.ViewUserState(name + " (이)가 열쇠를 완성했습니다.");
        }
    }

    /// <summary>
    /// 열쇠의 완성을 알립니다.
    /// </summary>
    /// <param name="have">완성했는가?</param>
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

        User user = enterRoom.FindUserByName(name);

        if (user != null)
        {
            user.isBoss = true;
            enterRoom.UpdateBossPlayer(1);
            UIInGame.instance.ViewNotice(name + " (이)가 첫 술래가 되었습니다.");
        }

    }

    /// <summary>
    /// 숙주 술래를 알립니다.
    /// </summary>
    /// <param name="name">술래 이름</param>
    public void SendRootTag(string name)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("tager", name);

        socket.Emit("rootTag", json);
    }

    public void OnChildTag(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string tager = json.GetField("tager").str;
        string name = json.GetField("child").str;

        User user = enterRoom.FindUserByName(name);

        if (user != null)
        {
            user.isBossChild = true;
            enterRoom.UpdateBossPlayer(1);
            UIInGame.instance.ViewUserState(name + " (이)가 " + tager + "에게 잡혔습니다.");
        }

    }

    /// <summary>
    /// 술래에게 태그당함을 알립니다.
    /// </summary>
    /// <param name="name">술래 이름</param>
    /// <param name="other">당한 사람 이름</param>
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

    /// <summary>
    /// 포탈을 생성합니다.
    /// </summary>
    /// <param name="pos">좌표</param>
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

        UIInGame.instance.ViewGameState(name + " (이)가 포탈을 열었습니다. 어서 들어가세요!");
        GameManager.instance.portal.Open();


    }

    /// <summary>
    /// 포탈을 엽니다.
    /// </summary>
    /// <param name="name">유저 이름</param>
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
        GameManager.instance.portal.Close();
        UIInGame.instance.ViewGameState("포탈이 닫혔습니다. 마지막까지 살아남으세요.");
    }

    /// <summary>
    /// 포탈을 닫습니다.
    /// </summary>
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
        int where = (int)json.GetField("where").f;



        GameObject g = GameObject.FindGameObjectWithTag("UIManager");
        switch (where)
        {
            case 0:
                enterRoom.FindUserByName(name).controller.infoUI.SetChatText(message);
                g.GetComponent<UILobby>().AddChatLog(name + " : " + message);
                break;
            case 1:
                enterRoom.FindUserByName(name).controller.infoUI.SetChatText(message);
                g.GetComponent<UIWaitRoom>().AddChatLog(name + " : " + message);
                break;
            case 2:
                Sprite icon = g.GetComponent<UIInGame>().GetEmotionIcon(int.Parse(message));
                enterRoom.FindUserByName(name).controller.infoUI.SetEmotion(icon);
                break;
        }

    }

    /// <summary>
    /// 채팅을 보냅니다.
    /// </summary>
    /// <param name="name">유저 이름</param>
    /// <param name="message">메세지</param>
    /// <param name="where">어디에서 보내는가? 0 = 로비  1 = 방 2 = 인 게임</param>
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
                Destroy(roomList[i].ui.gameObject);
                roomList.RemoveAt(i);
                break;
            }
        }
    }

    public GameObject MakePlayerInfoUI(Transform canvas, Transform tr, string name)
    {
        GameObject g = Instantiate(playerInfoUI, canvas);

        g.GetComponent<UITargetUserInfo>().SetTarget(tr, name);

        return g;
    }

    #endregion

}
