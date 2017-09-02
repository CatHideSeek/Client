using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirAnimation : MonoBehaviour {

    public float height = 1;

	// Update is called once per frame
	void Update () {
        transform.position += Vector3.up * Mathf.Sin(Time.time) * Time.deltaTime * height;
    }
}
