using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Trap : MonoBehaviour
{
    Rigidbody ri;
    BoxCollider col;
    string owner;

    void Start()
    {
        ri = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
    }

    void OnCollisionEnter(Collision c)
    {
        ri.useGravity = false;
        col.isTrigger = true;
    }


    /// <summary>
    /// 덫의 소유자를 설정합니다.
    /// </summary>
    /// <param name="name">유저 이름</param>
    public void SetOwner(string name)
    {
        owner = name;
    }


    /// <summary>
    /// 덫 소유자의 이름을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public string GetOwner()
    {
        return owner;
    }
}
