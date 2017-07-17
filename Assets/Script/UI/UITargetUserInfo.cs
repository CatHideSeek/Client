using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITargetUserInfo : MonoBehaviour
{
    public GameObject nameLabel, chatBuble, readyLabel, emotionBuble;

    public Text nameText, chatText;

    Transform target;
    RectTransform tr;

    public Image itemIcon, emotionIcon;

    public Vector2 margin;

    public float chatViewTime = 1f;
	public Image slow;
	public Image stun;

    void Awake()
    {
        tr = GetComponent<RectTransform>();
        tr.SetAsFirstSibling();
    }

    public void SetTarget(Transform targetTr, string name)
    {
        target = targetTr;
        nameText.text = name;
    }

    public void SetTarget(Transform targetTr, string name, Vector2 customMargin)
    {
        target = targetTr;
        nameText.text = name;
        margin = customMargin;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
            tr.position = new Vector2(screenPos.x + margin.x, screenPos.y + margin.y);
        }

        User u = target.GetComponent<PlayerController>().user;
		if (u.GetTeam() != PlayerDataManager.instance.my.GetTeam()||!u.controller.model.active)
        {
            nameLabel.SetActive(false);
        }
        else
        {
            nameLabel.SetActive(true);
        }

		stun.gameObject.transform.Rotate (new Vector3 (0, 0, Time.deltaTime * 180));
    }


    public void SetChatText(string msg) {
        chatText.text = msg;
        if (chatView != null) {
            StopCoroutine(chatView);
        }

        chatView = StartCoroutine(ChatBubleView());
    }

    public void SetItemIcon(Sprite icon, bool view) {
        itemIcon.sprite = icon;
        chatBuble.SetActive(view);
    }

    Coroutine chatView = null;

    IEnumerator ChatBubleView() {
        chatBuble.SetActive(true);
        yield return new WaitForSeconds(chatViewTime);

        chatText.text = "";
        chatBuble.SetActive(false);

        chatView = null;
    }

    public void SetRedayLabel() {
        readyLabel.SetActive(!(readyLabel.activeSelf));
    }

    public void SetEmotion(Sprite emotion) {
        emotionIcon.sprite = emotion;
        if (emotionView != null)
        {
            StopCoroutine(emotionView);
        }

        emotionView = StartCoroutine(EmotionBubleView());
    }

    Coroutine emotionView = null;

    IEnumerator EmotionBubleView()
    {
        emotionBuble.SetActive(true);
        yield return new WaitForSeconds(chatViewTime);

        emotionIcon.sprite = null;
        emotionBuble.SetActive(false);

        emotionView = null;
    }
}
