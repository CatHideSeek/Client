using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public float viewRange = 10;
    public GameObject arrow;
    PlayerController p;
    Renderer renderer;

    void Start()
    {
        p=GetComponent<PlayerController>();
        if(!p.user.isPlayer||PlayerDataManager.instance.modelType!=3)
            return;

        arrow = Instantiate(arrow, transform.position, arrow.transform.rotation);
        renderer = arrow.transform.Find("Model").Find("Cube").GetComponent<Renderer>();
    }

    void FixedUpdate()
    {
        if (!p.user.isPlayer || PlayerDataManager.instance.modelType != 3)
            return;
        arrow.transform.position = transform.position;

        GameObject[] taggedEnemys = GameObject.FindGameObjectsWithTag("Player");
        float dist=0;
        float closestDistSqr = Mathf.Infinity;//infinity 실제값?
        Transform closestEnemy = null;
        foreach (GameObject taggedEnemy in taggedEnemys)
        {
            PlayerController e = taggedEnemy.GetComponent<PlayerController>();
            if (taggedEnemy == PlayerDataManager.instance.my.controller.gameObject||e.user.GetTeam()==p.user.GetTeam())
                continue;

            Vector3 objectPos = taggedEnemy.transform.position;
            dist = (objectPos - transform.position).sqrMagnitude;
            //원주민이 특정 거리 안으로 들어올때
            if (dist < viewRange)
            {
                // 그 거리가 제곱한 최단 거리보다 작으면
                if (dist < closestDistSqr)
                {
                    closestDistSqr = dist;
                    closestEnemy = taggedEnemy.transform;
                }
            }
        }

        Color c = renderer.material.color;
        if (closestDistSqr < viewRange)
        {
            arrow.transform.LookAt(new Vector3(closestEnemy.transform.position.x, arrow.transform.position.y, closestEnemy.transform.position.z));
            c.a = 1;
            arrow.SetActive(true);
        }
        else
        {
            c.a = 0;
            arrow.SetActive(false);
        }
        renderer.material.color = c;
//        Debug.Log(c.a);
    }
}