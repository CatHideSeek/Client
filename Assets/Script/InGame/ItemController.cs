using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public float flySpeed = 2f, rotSpeed = 180f, traSpeed=4f, upSpeed=2f;

    float time = 0,originY;
    bool destroy=false;
    Material mat;

    void Awake()
    {
        originY = transform.position.y;
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        time += flySpeed*Mathf.PI * Time.deltaTime;
        transform.Translate(0, originY + Mathf.Sin(time) / 4 - transform.position.y, 0, Space.World);
        transform.Rotate(new Vector3(0, rotSpeed, 0) * Time.deltaTime);
        
        if (destroy)
        {
            Color c = mat.color;
            c.a -= traSpeed * Time.deltaTime;
            mat.color = c;
            originY += upSpeed * Time.deltaTime;
            if(c.a<=0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetDestroy()
    {
        destroy=true;
        flySpeed = 0;
        rotSpeed = 0;
    }

    public bool GetDestroy()
    {
        return destroy;
    }
}