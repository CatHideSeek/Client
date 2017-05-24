using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawner : MonoBehaviour
{
    public GameObject keyPrefab;
    public bool created = false;
    float spawnDelay=10f, spawnTime=0;
	
	void Update ()
    {
        if (!created)
            spawnTime -= Time.deltaTime;
        if(spawnTime<=0)
        {
            GameObject key=Instantiate(keyPrefab, transform.position + keyPrefab.transform.position, Quaternion.identity);
            key.GetComponent<Key>().spawner = this;
            spawnTime = spawnDelay;
            created=true;
        }
	}
}