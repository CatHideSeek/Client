using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] prefabs;
    public bool created = false;
    float spawnDelay=10f, spawnTime=0;
	
	void Update ()
    {
        if (!GameManager.instance.isPlay)
            return;
        if (!created)
            spawnTime -= Time.deltaTime;
        if(spawnTime<=0)
        {
            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            GameObject key=Instantiate(prefab, transform.position + prefab.transform.position, Quaternion.identity);
            key.GetComponent<Item>().spawner = this;
            spawnTime = spawnDelay;
            created=true;
        }
	}
}