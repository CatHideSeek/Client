﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {


    public static GameManager instance;

    public float currentTime = 120f, portalOpenTime = 60f;

    public bool isPlay = false;
    public bool isPortalSet = false;
    public bool isEnd = false;

    public GameObject playerObject, portalObject;
    public Transform map;

    public PortalController portal;

    UIInGame ui;

    void Awake() {
        instance = this;
    }

    void Start() {
        ui = UIInGame.instance;
    }

    public void StartGame() {
        isPlay = true;
    }
	
	// Update is called once per frame
	void Update () {

        if (isPlay)
            Timer();
       
	}

    void Timer() {
        currentTime -= Time.deltaTime;

        ui.UpdateTimerText(string.Format("{0:D2} : {1:D2}",(int)currentTime/60, (int)currentTime%60));


        //포탈 생성 조건
        if (!isPortalSet && currentTime < portalOpenTime) {
            portalObject.SetActive(true);
        }

        //게임 종료 조건
        if (!isEnd && currentTime < 0)
        {
            isEnd = true;
            currentTime = 0;
        }
    }


}
