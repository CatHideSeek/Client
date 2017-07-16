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
    public GameObject[] catModel;

    Transform tr;
    Rigidbody ri;
    Renderer renderer;

    public TagController tagController;

    [SerializeField]
    private float h, v;

    public float movSpeed = 5f, rotSpeed = 10f, jumpPower = 6f, opaSpeed = 8f;
    float oriMSpd, oriRSpd;

    Vector3 oldPos, currentPos;
    Quaternion oldRot, currentRot;
    float hideAlpha = 1;

    GameObject nameLabel;
    GameObject model;
    GameObject[] changeObjs;

    [SerializeField]
    float clampTime = 50f;

    public bool createdModel = false;

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

        oriMSpd = movSpeed;
        oriRSpd = rotSpeed;

        model = transform.Find("Cat").gameObject;
        changeObjs = new GameObject[3];
        changeObjs[0] = transform.Find("Tree").gameObject;
        changeObjs[1] = transform.Find("Frost").gameObject;
        changeObjs[2] = transform.Find("Rock").gameObject;
    }

    /// <summary>
    /// 플레이어의 모델을 지정해줍니다
    /// </summary>
    /// <param name="model">모델 id값(0~4)</param>
    public void SetModel(int id)
    {
        Debug.Log("SetModel()");
        GameObject g = Instantiate(catModel[id],transform.position+catModel[id].transform.position,Quaternion.identity);
        g.transform.parent = model.transform;
        createdModel = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (user.isPlayer)
        {
//#if UNITY_EDITOR
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            if (Input.GetKeyDown(KeyCode.Z))
            {
                Jump();
            }
//#endif
        }

        #region UpdateState
        if (user.FindState((int)User.State.Hide) >= 0)
        {
            SetRenderer(false);
        }
        //else if (hideAlpha < 1)
        //{
        //    hideAlpha = Mathf.Lerp(hideAlpha, 0.5f, Time.deltaTime * opaSpeed);
        //    SetAlphaWithChildren(hideAlpha);
        //}

        if (user.FindState((int)User.State.Stun) >= 0)
        {
            movSpeed = 0;
            rotSpeed = 0;
        }
        else if (user.FindState((int)User.State.Slow) >= 0)
        {
            movSpeed = oriMSpd / 2;
            rotSpeed = oriRSpd / 2;
        }
        else if (movSpeed != oriMSpd)
        {
            movSpeed = oriMSpd;
            rotSpeed = oriRSpd;
        }

        if (user.FindState((int)User.State.Change) >= 0)
        {
            model.SetActive(false);
            for (int i = 0; i < 3; i++)
            {
                if (i + 1 == user.objectKind)
                    changeObjs[i].SetActive(true);
                else
                    changeObjs[i].SetActive(false);
            }
            Debug.Log("sdfs" + user.objectKind);
        }
        else if (!model.activeSelf)
        {
            model.SetActive(true);
            for (int i = 0; i < 3; i++)
            {
                changeObjs[i].SetActive(false);
            }
        }
        #endregion
    }

    void FixedUpdate()
    {
        if (user.isPlayer && (h!=0 || v!=0 || ri.velocity.y!=0))
            Move();
        else if (h == 0 && v == 0)
            ri.velocity = new Vector3(0, ri.velocity.y, 0);

        if (!user.isPlayer && oldPos != tr.position)
        {
            tr.position = Vector3.Lerp(tr.position, currentPos, Time.deltaTime * clampTime);
        }

        if (!user.isPlayer && oldRot != tr.rotation)
        {
            tr.rotation = Quaternion.Lerp(tr.rotation, currentRot, Time.deltaTime * clampTime);
        }

    }


    public void SetAxis(float _h, float _v)
    {
        h = _h;
        v = _v;
    }

    /// <summary>
    /// 유저 데이터를 세팅합니다.
    /// </summary>
    /// <param name="u">User 정보</param>
    public void SetUser(User u)
    {
        u.controller = this;

        if (u.isPlayer)
        {
            CameraController.instance.target = tr;
        }

        user = u;

        nameLabel = UIInGame.instance.MakeNameLabel(tr, user.name);
    }

    /// <summary>
    /// 유저 삭제를 진행합니다.
    /// </summary>
    public void DeletUser()
    {
        Destroy(nameLabel);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 유저 타입을 변경 합니다. 타입에 따른 이동속도가 재 지정됩니다.
    /// </summary>
    public void UpdateUserType()
    {
        if (user.isBoss)
        {
            movSpeed = 7.5f;
        }
        else if (user.isBossChild)
        {
            movSpeed = 6.25f;
        }
        else
        {
            movSpeed = 5;
        }
    }

    public void Jump()
    {
        ri.velocity = new Vector3(ri.velocity.x, jumpPower, ri.velocity.z);
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

        ri.velocity = new Vector3(0, ri.velocity.y, 0);
        ri.velocity = new Vector3(inputDir.x * movSpeed, ri.velocity.y, inputDir.z * movSpeed);

        CheckPosition();
        CheckRotation();
    }

    /// <summary>
    /// 이동 여부를 체크 하여 패킷을 전송합니다.
    /// </summary>
    public void CheckPosition()
    {
        //이동 하였는지 체크 합니다.
        if (oldPos!=tr.position)
        {
            oldPos = tr.position;
            //이동 하였다고 socket 메세지 전송
            NetworkManager.instance.SendPosition(oldPos);
        }
    }

    /// <summary>
    /// 회전 여부를 체크 하여 패킷을 전송합니다.
    /// </summary>
    public void CheckRotation()
    {
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
        }
    }

    /// <summary>
    /// 투명도를 설정합니다.(이 오브젝트의 자식들까지 포함해서)
    /// </summary>
    /// <param name="alpha">활성화 여부</param>
    private void SetRenderer(bool active)
    {
        model.SetActive(active);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Hide"))
        {
            if (!user.isPlayer && renderer)
                renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 0f); //0투명 ~ 1 불투명
            else
                col.GetComponent<OpacityObject>().SetOpacity();
        }
        else if (col.CompareTag("Item"))
        {
            Item item = col.GetComponent<Item>();
            if (!item.GetDestroy())
            {
                print("아이템 획득");
                if (user == PlayerDataManager.instance.my)
                    item.SetDestroy(true);
                else
                    item.SetDestroy(false);
            }
        }
        else if (col.CompareTag("Trap"))
        {
            if (!col.GetComponent<Trap>().GetOwner().Equals(user.name))
            {
                PlayerDataManager.instance.SetStun(2);
                Destroy(col.gameObject);
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (user.isPlayer && Input.GetKeyDown(KeyCode.Space))
        {
            if (col.CompareTag("Portal"))
            {
                if (GameManager.instance.portal.isOpen)
                {
                    print("탈출");
                }
                else if (user.isKeyHave)
                {
                    print("열쇠를 사용하여 오픈");
                    NetworkManager.instance.SendOpen(user.name);
                }
            }
            else if (col.CompareTag("Item"))
            {
                /*
                ItemController item = col.GetComponent<ItemController>();
                if (!item.GetDestroy())
                {
                    print("아이템 획득");
                    col.GetComponent<ItemController>().SetDestroy();
                }
                */
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Hide"))
        {
            if (!user.isPlayer&&renderer)
                renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1f); //0투명 ~ 1 불투명
            else
                col.GetComponent<OpacityObject>().SetOpacity(false);
        }
    }

}