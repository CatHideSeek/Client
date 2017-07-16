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
        socket.On("initEnd", OnInitEnd);
        socket.On("trap", OnTrap);
        socket.On("generator", OnGenerator);
        socket.On("spawnPos", OnSpawnPos);
        socket.On("map", OnBlock);
        socket.On("portalCreate", OnPortal);
        socket.On("portalOpen", OnOpen);
        socket.On("portalClose", OnClose);
        #endregion

        #region etc
        socket.On("chat", OnChat);
        #endregion


    }

    public void TestEnterRoom()
    {
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

    /// <summary>
    /// 서버에게 방 리스트를 요청합니다.
    /// </summary>
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

    /// <summary>
    /// 서버로 방 생성을 전송합니다.
    /// </summary>
    /// <param name="name">방 이름</param>
    /// <param name="maxPlayers">최대 입장 가능 수</param>
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

        //bool.TryParse(json.GetField("isHost").str, out playerData.my.isHost);

        //입장한 방 세팅
        //enterRoom = FindRoom(name);
        //씬 이동
        //씬 ~ 이 ~ 동 ~ 코 ~ 드 ~

    }

    /// <summary>
    /// 서버로 방 입장 시도를 보냅니다.
    /// </summary>
    /// <param name="name">방 이름</param>
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
        bool isHost = json.GetField("isHost").b;


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

        UIInGame.instance.ViewNotice(user.name + "이 참가하였습니다.");

        if (PlayerDataManager.instance.my.isHost&&!PlayerDataManager.instance.my.name.Equals(user.name))
        {
            SendSpawnPos(user.name, GameManager.instance.spawnPos.y);
            foreach(IslandInfo info in GameManager.instance.islandList)
            {
                SendGenerator(user.name,info.x, info.y, info.id);//생성기 먼저 생성해야됨
            }
            foreach (Block block in GameManager.instance.blockList)
            {
                SendBlock(user.name, block.pos, block.id,block.parent);
            }
            SendInitEnd(user.name);
        }

        SendPosition(PlayerDataManager.instance.my.controller.transform.position);//방금 입장한 사람을 위해 현재좌표를 전송
    }

    public void OnBlock(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string targetName = json.GetField("target").str;
        if (targetName.Equals(Define.ALL_Target))
            targetName = PlayerDataManager.instance.my.name;

        //처음 들어온 유저에게만 맵을 생성하게 하기 위해
        if (PlayerDataManager.instance.my.name.Equals(targetName))
        {
            float x = json.GetField("bx").f;
            float y = json.GetField("by").f;
            float z = json.GetField("bz").f;
            int id = (int)json.GetField("bid").f;
            int parent = (int)json.GetField("parent").f;
            Debug.Log(parent);
            GameObject block=Instantiate(GameManager.instance.blockObject[id], new Vector3(x, y, z), GameManager.instance.blockObject[id].transform.rotation);
            if (id == 6 || id == 7)
                block.transform.parent = MapGenerator.instance.transform;
            else
            {
                Transform parentIsland=GameObject.Find("Island " + parent).transform;
                block.transform.parent = parentIsland;
                parentIsland.GetComponent<IslandGenerator>().blocks.Add(block);
            }
            //오브젝트를 받아와서 배치함(id값은 0~3:땅, 4: 나무, 5: 수풀, 6~7: 다리, 8:키스포너)
        }
    }


    /// <summary>
    /// 블럭의 좌표를 다른 클라이언트에게 보냅니다.
    /// </summary>
    /// <param name="targetName">유저 이름</param>
    /// <param name="pos">위치 Vector3</param>
    /// <param name="id">블럭 종류</param>
    /// <param name="parent">부모</param>
    public void SendBlock(string targetName, Vector3 pos, int id,int parent=-1)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("target", targetName);
        json.AddField("bx", pos.x);
        json.AddField("by", pos.y);
        json.AddField("bz", pos.z);
        json.AddField("bid", id);
        json.AddField("parent", parent);

        socket.Emit("map", json);
    }

    public void OnInitEnd(SocketIOEvent e)
    {
        JSONObject json = e.data;

        if (!PlayerDataManager.instance.my.name.Equals(json.GetField("target").str))
            return;
        for (int i = 0; i < MapGenerator.instance.islandNum;i++)
        {
            //여기서 병합해야됨,
            //Debug.Log("sdfsdf"+MapGenerator.instance.islandNum);
            //GameObject.Find("Island " + i).transform.GetComponent<IslandGenerator>().Combine();
        }

        MapGenerator.instance.transform.Rotate(new Vector3(0, 45, 0));
    }

    /// <summary>
    /// 맵생성이 끝낫음을 알려준다
    /// </summary>
    public void SendInitEnd(string targetName)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("target", targetName);
        socket.Emit("initEnd", json);
    }

    public void OnGenerator(SocketIOEvent e)
    {
        JSONObject json = e.data;

        if (!PlayerDataManager.instance.my.name.Equals(json.GetField("target").str))
            return;

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
    public void SendGenerator(string targetName,int x,int y,int id)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("target", targetName);
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

        if (owner.Equals(PlayerDataManager.instance.my.name))
            return;

        Trap t=Instantiate(GameManager.instance.blockObject[9], new Vector3(x, y, z), GameManager.instance.blockObject[9].transform.rotation).GetComponent<Trap>();
        t.SetOwner(owner);
    }

    /// <summary>
    /// 트랩생성 정보를 다른 클라이언트에게 보냅니다.
    /// </summary>
    /// <param name="pos"></param>
    public void SendTrap(Vector3 pos)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("x", pos.x);
        json.AddField("y", pos.y);
        json.AddField("z", pos.z);
        json.AddField("owner", PlayerDataManager.instance.my.name);

        socket.Emit("trap", json);
    }

    public void OnSpawnPos(SocketIOEvent e)
    {
        JSONObject json = e.data;

        if (PlayerDataManager.instance.my.name.Equals(json.GetField("target").str) && !PlayerDataManager.instance.my.isHost)
        {
            GameManager.instance.spawnPos.y = json.GetField("sy").f;
            PlayerDataManager.instance.my.controller.transform.position = GameManager.instance.spawnPos;
        }
    }


    /// <summary>
    /// 스폰좌표를 다른 클라이언트에게 보냅니다.
    /// </summary>
    /// <param name="target">타겟</param>
    /// <param name="y">y 좌표</param>
    public void SendSpawnPos(string target, float y)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("target", target);
        json.AddField("sy", y);
        print("SendSpawnPos");

        socket.Emit("spawnPos", json);
    }

    /// <summary>
    /// 서버로 방 입장에 성공했음을 보냅니다.
    /// </summary>
    public void SendJoin()
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        playerData.my.name = "User" + Random.Range(1, 100);

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
            string socketID = json["userList"][i]["socketID"].ToString();
            string name = json["userList"][i]["name"].ToString();
            bool isHost = bool.Parse(json["userList"][i]["isHost"].ToString());


            if (playerData.my.name != name)
            {

                User user = new User(name, socketID);
                user.isHost = isHost;

                enterRoom.AddUser(user);

                GameObject g = Instantiate(GameManager.instance.playerObject, GameManager.instance.spawnPos, Quaternion.identity);
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
        GameManager.instance.StartGame();

        print("game is play");
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

    /// <summary>
    /// 서버로 방 나가기를 보냅니다.
    /// </summary>
    public void SendExit()
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("a", 0);

        socket.Emit("roomExit", json);

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

        User user = enterRoom.FindUserByName(name);


        if (user != null)
        {
            user.controller.SetPosition(new Vector3(x, y, z));
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

        if(state>0)
        {
            if (user.FindState(state) == -1)
            {
                user.PushState((User.State)state);
                if (state == (int)User.State.Change)
                    user.objectKind = objKind;
                Debug.Log("OnState()" + user.FindState(state));
            }
        }
        else if(state<0)
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
    public void SendState(int state,int objKind=0)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("name", playerData.my.name);
        json.AddField("state", state);
        json.AddField("objKind", objKind);

        socket.Emit("state", json);

        Debug.Log("SendState()");
    }

    public void OnGetKeyPart(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("name").str;

        User user = enterRoom.FindUserByName(name);

        if (user != null)
        {
            user.keyCount = (int)json.GetField("part").f;
            UIInGame.instance.ViewNotice(name + " (이)가 열쇠 조각을 얻었습니다.");
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
            UIInGame.instance.ViewNotice(name + " (이)가 열쇠를 완성했습니다.");
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
            UIInGame.instance.ViewNotice(name + " (이)가 " + tager + "에게 잡혔습니다.");
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
        UIInGame.instance.ViewNotice("포탈이 생성되었습니다.");
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

        UIInGame.instance.ViewNotice(name + " (이)가 포탈을 열었습니다. 어서 들어가세요!");
        GameManager.instance.portal.isOpen = true;

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
        UIInGame.instance.ViewNotice("포탈이 닫혔습니다. 마지막까지 살아남으세요.");
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
        int where = 0;
        int.TryParse(json.GetField("where").str, out where);
    }

    /// <summary>
    /// 채팅을 보냅니다.
    /// </summary>
    /// <param name="name">유저 이름</param>
    /// <param name="message">메세지</param>
    /// <param name="where">어디에서 보내는가?</param>
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
