using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITargetUserName : MonoBehaviour
{

    public Text nameText;

    Transform target;
    RectTransform tr;

    public Vector2 margin;

    void Awake()
    {
        tr = GetComponent<RectTransform>();
        tr.SetAsFirstSibling();
    }

    public void SetTarget(Transform targetTr,string name)
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
    }

}
