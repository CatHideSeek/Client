using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITargetUserName : MonoBehaviour
{

    public Text nameText;

    Transform target;
    RectTransform tr;
    string targetName;
    Image image;

    public Vector2 margin;

    void Awake()
    {
        tr = GetComponent<RectTransform>();
        tr.SetAsFirstSibling();
        image = GetComponent<Image>();
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
        targetName = name;
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
        Debug.Log(u.GetTeam() + "   " + PlayerDataManager.instance.my.GetTeam());
        if (u.GetTeam() != PlayerDataManager.instance.my.GetTeam())
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            nameText.color = new Color(nameText.color.r, nameText.color.g, nameText.color.b, 0);
        }
        else
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            nameText.color = new Color(nameText.color.r, nameText.color.g, nameText.color.b, 1);
        }
    }

}
