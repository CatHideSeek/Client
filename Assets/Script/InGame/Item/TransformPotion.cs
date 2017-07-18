using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformPotion : Item
{
    protected override void Eaten()
    {
        PlayerDataManager.instance.EatItem(3);
        spawner.created = false;
    }
}
