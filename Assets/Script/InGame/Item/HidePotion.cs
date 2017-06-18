using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidePotion : Item {

    protected override void Eaten()
    {
        PlayerDataManager.instance.EatItem(1);
    }
}
