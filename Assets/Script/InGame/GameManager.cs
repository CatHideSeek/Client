using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float currentTime = 120f, portalOpenTime = 60f;


    public bool isPlay = false;
    public bool isPortalSet = false;
    public bool isEnd = false;

    public MapGenerator mapGenerator;
    public GameObject playerObject, portalObject;
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

    public PortalController portal;
    public int keySpawnerNum;

    UIInGame ui;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ui = UIInGame.instance;
    }

    public void StartGame()
    {
        isPlay = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlay && !isEnd)
            Timer();
    }

    void Timer()
    {
        currentTime -= Time.deltaTime;

        ui.UpdateTimerText(string.Format("{0:D2} : {1:D2}", (int)currentTime / 60, (int)currentTime % 60));


        //포탈 생성 조건
        if (!isPortalSet && currentTime < portalOpenTime&&portalObject)
        {
            portalObject.SetActive(true);
            isPortalSet = true;
        }

        //게임 종료 조건
        if (!isEnd && currentTime < 0)
        {
            isEnd = true;
            currentTime = 0;
        }
    }




}
