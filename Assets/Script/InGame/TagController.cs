using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 술래를 태그 할 수 있는 컨트롤러 입니다.
/// </summary>
public class TagController : MonoBehaviour
{
    [SerializeField]
    PlayerController player;

    bool isDelayTag = false;

    [SerializeField]
    bool isTag = false;

    void OnTriggerStay(Collider col)
    {
        //플레이어의 연결, 사용자 인가? , 스페이스바를 눌렀는가?
        if (player != null && player.user.isPlayer && UIActionButton.instance.tagPress)
        {
            //태그 딜레이가 실행중이지 않은가? , 태그 공격이 가능한 유저인가? , 태그가 충돌하고 있는가
            if (!isDelayTag && player.user.CheckTag() && !col.GetComponent<TagController>().player.user.CheckTag() && col.CompareTag("Tag") && !col.GetComponent<TagController>().player.clear)
            {
                NetworkManager.instance.SendChildTag(player.user.name, col.GetComponent<TagController>().player.user.name);
                player.PlayTagUser();
                StartCoroutine("DelayTag");
            }
        }
    }



    IEnumerator DelayTag()
    {
        isDelayTag = true;
        print("태그 실행");
        yield return new WaitForSeconds(1f);
        isDelayTag = false;
    }

}
