using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour {

    public PortalController portal;

	// Use this for initialization
	void Start () {
        NetworkManager.instance.SendLobbyEnter();
        SoundManager.instance.PlayLobbyBGM();
        StartCoroutine(TitleAnimation());
    }
    
    IEnumerator TitleAnimation()
    {
        float times = 0;
        while (true)
        {
            if ((int)times == 20)
            {
                portal.Open();
            }
            else if ((int)times == 40)
            {
                portal.CloseScrue();
                times = 0;
            }

            times += Time.deltaTime;
            yield return null;
        }
    }

}
