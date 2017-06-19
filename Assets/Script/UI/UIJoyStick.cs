using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIJoyStick : MonoBehaviour , IPointerDownHandler, IDragHandler , IPointerUpHandler
{
    [SerializeField]
    PlayerDataManager player;

    public RectTransform stick;

    [SerializeField]
    float radius;

    [SerializeField]
    Vector3 centerPos = Vector3.zero;

    Vector3 touch;


    private void Awake()
    {
        radius = GetComponent<RectTransform>().sizeDelta.x / 2;
    }

    private void Start()
    {
        player = PlayerDataManager.instance;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        stick.localPosition = Vector3.zero;
        centerPos = stick.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (stick == null)
            return;

#if UNITY_EDITOR
       touch = Input.mousePosition;
#else
        for (int i = 0; i < Input.touchCount; i++) {
            if (Camera.main.ScreenToViewportPoint(Input.GetTouch(i).position).x < 0.5f)
            {
                touch = Input.GetTouch(i).position;
            }
        }
#endif
        Vector3 dir = (new Vector3(touch.x, touch.y, centerPos.z) - centerPos).normalized;

        float touchAreaRadius = Vector3.Distance(centerPos, new Vector3(touch.x, touch.y, centerPos.z));
        if (touchAreaRadius > radius)
        {
            //반경을 넘어가는 경우는, 현재 가려는 방향으로, 반지름 만큼만 가도록 설정한다.
            stick.position = centerPos + (dir * radius);
        }
        else
        {
            //조이스틱이 반경내로 움직일때만, 드래그 된 위치로 설정한다.
            stick.position = touch;
        }

        player.my.controller.SetAxis(Mathf.Clamp(((stick.position.x - centerPos.x) / radius), -1f, 1f), Mathf.Clamp(((stick.position.y - centerPos.y) / radius), -1f, 1f));

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (stick != null)
        {
            stick.localPosition = Vector3.zero;
            centerPos = stick.position;
            player.my.controller.SetAxis(0, 0);
        }
    }
}
