using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActionButton : MonoBehaviour {

    PlayerDataManager player;

    public Image itemButtonIcon;

    private void Start()
    {
        player = PlayerDataManager.instance;
    }

    public void JumpButton() {
        player.my.controller.Jump();
    }

    public void ActionButton() {
        print("action button works");
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
        PlayerDataManager.instance.itemType = 0;
    }


}
