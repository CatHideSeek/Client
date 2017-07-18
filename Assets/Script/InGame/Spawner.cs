using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] prefabs;
    public bool created = false;
    public int s = 0;
    public float spawnDelay=10f;
    float spawnTime=0;
	
	void Update ()
    {
        if (!GameManager.instance.isPlay)
            return;
        if (!created)
            spawnTime -= Time.deltaTime;
        if(spawnTime<=0)
        {
            GameObject prefab = prefabs[s++];
            if (s >=prefabs.Length)
                s = 0;
            GameObject key=Instantiate(prefab, transform.position + prefab.transform.position, Quaternion.identity);
            key.GetComponent<Item>().spawner = this;
            spawnTime = spawnDelay;
            created=true;
        }

		if (PlayerDataManager.instance.my.GetTeam () == true) {
			gameObject.SetActive (false);
		}
	}
}