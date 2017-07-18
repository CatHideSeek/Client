using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightObject : MonoBehaviour {
	Transform player=null;
	public Renderer renderer;
	PlayerController pc;

	// Use this for initialization
	void Start () {
		pc = GetComponent<PlayerController> ();
		if (transform.Find ("Model"))
			renderer = transform.Find ("Model").GetComponent<Renderer> ();
		else {
			if (pc)
				renderer = null;
			else
				renderer = GetComponent<Renderer> ();
		}
	}

	// Update is called once per frame
	void Update () {
		if (player == null) {
			if(GameManager.instance != null && GameManager.instance.isModelReady)
				player = PlayerDataManager.instance.my.controller.gameObject.transform;
			return;
		}
		if (Vector3.Distance (player.position, transform.position) <= 4f) { // distance 사용 금지 
			if (pc)
				pc.model.SetActive (true);
			else
				renderer.enabled = true;
		} else {
			if (pc)
				pc.model.SetActive (false);
			else
				renderer.enabled = false;
		}
	}
}
