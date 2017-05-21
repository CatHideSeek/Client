using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {


    public static GameManager instance;

    public float currentTime = 150f;

    public bool isPlay = false;
    public bool isEnd = false;

    public GameObject player;

    void Awake() {
        instance = this;
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
        if (isEnd == false && currentTime < 0)
        {
            isEnd = true;
            currentTime = 0;
        }
    }

}
