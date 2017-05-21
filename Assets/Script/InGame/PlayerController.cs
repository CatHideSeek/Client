using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인게임 내에서 유저를 컨트롤 하는 클래스입니다.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

    public User user;

    Transform tr;
    Rigidbody ri;
    Renderer renderer;


    private float h, v;

    public float movSpeed = 10f, rotSpeed = 5f;


    Vector3 oldPos, currentPos;
    Quaternion oldRot, currentRot;


    void Awake()
    {

        tr = GetComponent<Transform>();
        ri = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();

        //위치 초기값 지정
        currentPos = tr.position;
        oldPos = currentPos;

        //회전 초기값 지정
        currentRot = tr.rotation;
        oldRot = currentRot;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (user.isPlayer)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
        }

    }

    void FixedUpdate()
    {
        if (user.isPlayer && (h != 0 || v != 0))
            Move();
        else if (h == 0 && v == 0)
            ri.velocity = new Vector3(0, ri.velocity.y, 0);
    }

    /// <summary>
    /// 유저 데이터를 세팅합니다.
    /// </summary>
    /// <param name="u">User 정보</param>
    public void SetUser(User u)
    {
        user = u;
        u.controller = this;
        if (user.isPlayer)
            CameraController.instance.target = tr;
    }

    /// <summary>
    /// 플레이어의 이동 함수
    /// </summary>
    void Move()
    {

        Vector3 targetDir = Vector3.zero;
        Vector3 inputDir = new Vector3(h, 0, v);
        Vector3 pos = inputDir.normalized;
        pos.x = Mathf.Round(pos.x);
        pos.z = Mathf.Round(pos.z);

        if (pos.sqrMagnitude > 0.1f)
            targetDir = pos.normalized;

        tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * rotSpeed);

        ri.velocity = Vector3.zero;
        ri.AddForce(inputDir * movSpeed, ForceMode.VelocityChange);

        CheckPosition();
        CheckRotation();

    }

    public void CheckPosition() {
        //이동 하였는지 체크 합니다.
        if (oldPos != tr.position)
        {
            oldPos = tr.position;
            //이동 하였다고 socket 메세지 전송
            NetworkManager.instance.SendPosition(oldPos);
        }
    }

    public void CheckRotation() {
        //회전 하였는지 체크합니다.
        if (oldRot != tr.rotation)
        {
            oldRot = tr.rotation;
            //회전 하였다고 socket 메세지 전송
            NetworkManager.instance.SendRotation(oldRot);
        }
    }

    /// <summary>
    /// 다른 플레이어의 위치를 세팅합니다.
    /// </summary>
    /// <param name="pos">위치 Vector3</param>
    public void SetPosition(Vector3 pos)
    {
        currentPos = pos;
        //이동 하였는지 체크 합니다.
        if (oldPos != currentPos)
        {
            tr.position = currentPos;
            oldPos = currentPos;
        }
    }

    /// <summary>
    /// 다른 플레이어의 회전을 세팅합니다.
    /// </summary>
    /// <param name="rot">회전 Quaternion</param>
    public void SetRotation(Quaternion rot)
    {
        currentRot = rot;
        //회전 하였는지 체크합니다.
        if (oldRot != currentRot)
        {
            oldRot = currentRot;
            tr.rotation = oldRot;
        }
    }

    void OnTriggerEnter(Collider col) {
        if (col.CompareTag("Hide")) {
            renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 0f); //0투명 ~ 1 불투명
        }
    }

    void OnTriggerStay(Collider col) {
        if (Input.GetKeyDown(KeyCode.Space) && col.CompareTag("Portal")) {
            if (GameManager.instance.portal.isOpen) {
                print("탈출");
            }
            else {
                print("열쇠를 사용하여 오픈");
            }
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.CompareTag("Hide"))
        {
            renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1f); //0투명 ~ 1 불투명
        }
    }

}
