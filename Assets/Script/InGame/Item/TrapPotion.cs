using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapPotion : Item
{
    protected override void Eaten()
    {
        PlayerDataManager.instance.EatItem(2);
        spawner.created = false;
    }
}
