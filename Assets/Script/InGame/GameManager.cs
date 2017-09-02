using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float currentTime = 130f, portalOpenTime = 60f;


    public bool isPlay = false;
    public bool isPortalSet = false;
    public bool isEnd = false;

	public bool isModelReady=false;

    public MapGenerator mapGenerator;
	public GameObject playerObject;
	public GameObject portalObject;
    public GameObject[] blockObject;
    //0~3 땅블럭
    //4 나무
    //5 수풀
    //6~7 다리
    //8 키스포너
    //9 트랩
    public Vector3 spawnPos = Vector3.zero;
    public List<Block> blockList = new List<Block>();
    public Transform map;
    public List<IslandInfo> islandList = new List<IslandInfo>();
    public List<Vector3> spawnList = new List<Vector3>();

    public PortalController portal;
    public int keySpawnerNum;
	public int portalIsland=-1;
	public bool isEscape=false;

    UIInGame ui;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ui = UIInGame.instance;
        SoundManager.instance.PlayInGameBGM();
    }

    public void StartGame()
    {
        UILoadFade.instance.fading = true;
        NetworkManager.instance.enterRoom.SetGravity();
        UIInGame.instance.ViewNotice("10초 후에 술래가 결정됩니다!");
        switch (PlayerDataManager.instance.modelType)
        {
            case 0:
                UIInGame.instance.ViewGameState("이 고양이는 가장 멍청합니다.");
                break;
            case 1:
                UIInGame.instance.ViewGameState("이 고양이는 더 오래 은신할 수 있습니다.");
                break;
            case 2:
                UIInGame.instance.ViewGameState("이 고양이는 변신 상태에서 이동할 수 있습니다.");
                break;
            case 3:
                UIInGame.instance.ViewGameState("이 고양이는 가까운 적을 탐지할 수 있습니다.");
                break;
            case 4:
                UIInGame.instance.ViewGameState("이 고양이의 덫은 적을 기절시킬 수 있습니다.");
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (UILoadFade.instance.fading && !isEnd)
            Timer();
    }

	void Timer()
	{

		if(!isEnd)
			currentTime -= Time.deltaTime;

		ui.UpdateTimerText(string.Format("{0:D2} : {1:D2}", (int)currentTime / 60, (int)currentTime % 60));

        if(currentTime<=120f&&!isPlay)
        {
            isPlay = true;
            if (PlayerDataManager.instance.my.isHost)
                NetworkManager.instance.enterRoom.SetFirstBoss();

            
        }

		//포탈 생성 조건
		if (!isPortalSet && currentTime < portalOpenTime&&portalObject)
		{
            UIInGame.instance.ViewGameState("포탈이 생성되었습니다.");            
			portalObject.SetActive(true);
            portal.transform.rotation = Quaternion.identity;
			isPortalSet = true;
		}

		//게임 종료 조건
        if (currentTime < 0)
        {
            currentTime = 0;
            EndGame();
        }
        else if (checkPlayer() == 0)
            EndGame();
	}

	public void EndGame()
	{
		isEnd = true;
		bool manWin;

        if (currentTime <= 0)//시간이 다되서 끝낫을때
        {
            manWin = checkPlayerWin();
        }
        else//유저가 모두 잡히거나, 모두  탈출
        {
                manWin = GameManager.instance.isEscape;
        }


		User myu=PlayerDataManager.instance.my;

		if (myu.GetTeam ()) {//좀비임
			UIInGame.instance.SetResult(manWin,manWin);
		}
		else
		{
			UIInGame.instance.SetResult(!manWin,manWin);
		}
	}

	private bool checkPlayerWin()
	{
		int w=0,l=0;
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < players.Length; i++)
		{
			if(players[i].GetComponent<PlayerController>().user.GetTeam())
				continue;

			if(players[i].GetComponent<PlayerController>().clear)
				w++;
			else
				l++;
		}

		if (w >= l && w > 0)
			return true;
		else
			return false;
	}


    private int checkPlayer()
    {
        int s = 0;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerController>().clear)
                continue;
            if (!players[i].GetComponent<PlayerController>().user.GetTeam())
                s++;
        }
        return s;
    }

    private int checkTager()
    {
        int s = 0;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerController>().user.GetTeam())
                s++;
        }
        return s;
    }
}
