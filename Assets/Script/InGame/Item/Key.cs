using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Item 
{
    public KeySpawner spawner;

    protected override void Eaten()
    {
        PlayerDataManager.instance.EatKey();
        spawner.created = false;
    }
}