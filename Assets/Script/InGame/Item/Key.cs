using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Item 
{

    protected override void Eaten()
    {
        PlayerDataManager.instance.EatKey();
        spawner.created = false;
    }
}