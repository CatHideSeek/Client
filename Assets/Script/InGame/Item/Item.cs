﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float flySpeed = 2f, rotSpeed = 180f, traSpeed=2f, upSpeed=2f;
    public Spawner spawner;

    float time = 0,originY;
    bool destroy=false;
    Material[] mat;

    void Awake()
    {
        originY = transform.position.y;
        mat = transform.Find("Model").GetComponent<Renderer>().materials;
    }

    void Update()
    {
        time += flySpeed*Mathf.PI * Time.deltaTime;
        transform.Translate(0, originY + Mathf.Sin(time) / 4 - transform.position.y, 0, Space.World);
        transform.Rotate(new Vector3(0, rotSpeed, 0) * Time.deltaTime);
        
		if (destroy) {
			Color c = mat [0].color;
			c.a -= traSpeed * Time.deltaTime;
			for (int i = 0; i < mat.Length; i++)
				mat [i].color = c;
			originY += upSpeed * Time.deltaTime;
			if (c.a <= 0) {
				Destroy (gameObject);
			}
		} else {
			if (PlayerDataManager.instance.my.GetTeam () == true) {
				Destroy (gameObject);
			}
		}
    }

    protected virtual void Eaten(){}

    public void SetDestroy(bool user)
    {
        if(user)
            Eaten();
        destroy=true;
        flySpeed = 0;
        rotSpeed = 0;
    }

    public bool GetDestroy()
    {
        return destroy;
    }
}