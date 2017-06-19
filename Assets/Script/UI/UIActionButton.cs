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
        switch(PlayerDataManager.instance.itemType)
        {
            case 1:
                PlayerDataManager.instance.SetHide(3);
                break;
            case 2:
                break;
            default:
                break;
        }
        PlayerDataManager.instance.itemType = 0;
    }


}
