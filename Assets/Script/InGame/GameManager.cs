using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {


    public static GameManager instance;

    public float currentTime = 150f;

    public bool isPlay = false;
    public bool isPortalSet = false;
    public bool isEnd = false;

    public GameObject playerObject, portalObject;
    public Transform map;

    public PortalController portal;

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
        if (!isPortalSet && currentTime < 75f) {
            //GameObject g = Instantiate(portal,map);
            portalObject.SetActive(true);
        }

        if (!isEnd && currentTime < 0)
        {
            isEnd = true;
            currentTime = 0;
        }
    }

}
