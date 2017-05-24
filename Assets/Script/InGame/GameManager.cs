using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {


    public static GameManager instance;

<<<<<<< HEAD
    public float currentTime = 120f, portalOpenTime = 60f;
=======
    public float currentTime = 120f;
>>>>>>> re

    public bool isPlay = false;
    public bool isPortalSet = false;
    public bool isEnd = false;

    public GameObject playerObject, portalObject;
    public Transform map;

    public PortalController portal;
    public GameObject keySpawnerPrefab;
    public int keySpawnerNum;

    UIInGame ui;

    void Awake() {
        instance = this;
    }

    void Start() {
        ui = UIInGame.instance;
    }

    public void StartGame() {
        isPlay = true;
        SetKeySpawner();
    }

    void SetKeySpawner()
    {
        for (int i = 0; i < keySpawnerNum; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(-20f, 20f), 0, Random.Range(-20f, 20f));
            Instantiate(keySpawnerPrefab, randomPos, Quaternion.identity);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (isPlay)
            Timer();
<<<<<<< HEAD
       
=======
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
>>>>>>> re
	}

    void Timer() {
        currentTime -= Time.deltaTime;
<<<<<<< HEAD

        ui.UpdateTimerText(string.Format("{0:D2} : {1:D2}",(int)currentTime/60, (int)currentTime%60));


        //포탈 생성 조건
        if (!isPortalSet && currentTime < portalOpenTime) {
=======
        if (!isPortalSet && currentTime < 60f) {
            //GameObject g = Instantiate(portal,map);
>>>>>>> re
            portalObject.SetActive(true);
            isPortalSet = true;
        }

        //게임 종료 조건
        if (!isEnd && currentTime < 0)
        {
            isEnd = true;
            currentTime = 0;
        }
        print("currentTime: " + currentTime);
    }


}
