using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActionButton : MonoBehaviour {

    PlayerDataManager player;

    public static UIActionButton instance;
	public GameObject actionButton;

    public Image[] itemButtonIcon;
    public bool tagPress = false;

    float time = 0.1f;

    private void Start()
    {
        instance = this;
        player = PlayerDataManager.instance;
    }

    void Update()
    {
        if (time <= 0.1f)
        {
            time += Time.deltaTime;
            tagPress = true;
        }
        else
            tagPress = false;

		if (actionButton && PlayerDataManager.instance.my.GetTeam () == true)
			actionButton.SetActive (false);
    }

    public void JumpButton() {
        player.my.controller.Jump();
    }

    public void ActionButton() {
        print("action button works");
        if (PlayerDataManager.instance.my.controller.clear)
        {
            GameObject targetP = null;
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            do
            {
                targetP = players[Random.Range(0, players.Length)];
            } while (targetP.GetComponent<PlayerController>().clear);

            CameraController.instance.target = targetP.transform;
        }
        else
            time = 0;
    }

    public void UpdateItemIcon(Sprite icon) {

    }

    public void ItemButton() {
        print("item button works");
        switch(player.itemType)
        {
            case 1:
                if(player.modelType==1)
                    PlayerDataManager.instance.SetHide(6);
                else
                    PlayerDataManager.instance.SetHide(3);
                break;
            case 2:
                if (player.modelType == 4)
                    PlayerDataManager.instance.CreateTrap(true);
                else
                    PlayerDataManager.instance.CreateTrap(false);
                break;
            case 3:
                PlayerDataManager.instance.SetChange(4,Random.Range(1,4));
                break;
            default:
                break;
        }
		SetItem (0);
    }

	public void SetItem(int id)
	{
		PlayerDataManager.instance.itemType=id;
		for (int i = 0; i < itemButtonIcon.Length; i++) {
			itemButtonIcon [i].enabled = false;
		}
		if(id>0)
			itemButtonIcon [id-1].enabled = true;
	}
}
