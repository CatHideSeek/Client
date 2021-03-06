﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpacityObject : MonoBehaviour {
    [SerializeField]
    float opaSpeed = 8f;

    public bool opacity=false;
    Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Color c = mat.color;
        if(opacity)
        {
            c.a = Mathf.Lerp(c.a, 0.2f,Time.deltaTime * opaSpeed);
            mat.color = c;
        }
        else if(c.a<1)
        {
            c.a = Mathf.Lerp(c.a, 1f, Time.deltaTime * opaSpeed);
            mat.color = c;
        }
	}

    public void SetOpacity(bool _opacity=true)
    {
        opacity = _opacity;
    }

}