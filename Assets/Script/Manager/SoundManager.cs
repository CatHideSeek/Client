using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;

    public AudioClip titleBGM, lobbyBGM, waitRoomBGM, inGameBGM;

    public AudioClip getItemBGS, jumpBGS, tagBGS, noticeBGS, buttonBGS;

    AudioSource audioBGM;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        audioBGM = GetComponent<AudioSource>();
    }

    public void PlayTitleBGM()
    {
        audioBGM.Stop();
        audioBGM.clip = titleBGM;
        audioBGM.loop = true;
        audioBGM.Play();
    }

    public void PlayLobbyBGM()
    {
        audioBGM.Stop();
        audioBGM.clip = lobbyBGM;
        audioBGM.loop = true;
        audioBGM.Play();
    }

    public void PlayWaitRoomBGM()
    {
        audioBGM.Stop();
        audioBGM.clip = waitRoomBGM;
        audioBGM.loop = true;
        audioBGM.Play();
    }

    public void PlayInGameBGM()
    {
        audioBGM.Stop();
        audioBGM.clip = inGameBGM;
        audioBGM.loop = true;
        audioBGM.Play();
    }

    public void PlayNoticeBGS() {
        audioBGM.PlayOneShot(noticeBGS);
    }

    public void PlayButtonBGS() {
        audioBGM.PlayOneShot(buttonBGS);
    }





}
