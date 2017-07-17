using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public static CameraController instance;

    Camera cam;

    Transform tr;

    public Transform target;

    public Vector3 margin;
    public float followSpeed = 5f;

    void Awake()
    {
        instance = this;
        tr = GetComponent<Transform>();
        cam = GetComponent<Camera>();
    }

    /// <summary>
    /// 시야를 설정합니다. 클수록 멀리 보입니다.
    /// </summary>
    /// <param name="view">float 값</param>
    public void SetViewSize(float view)
    {
        if (viewSizeLerp != null)
            StopCoroutine(viewSizeLerp);

        viewSizeLerp = StartCoroutine(ViewSizeLerp(view));
    }

    Coroutine viewSizeLerp = null;

    IEnumerator ViewSizeLerp(float view)
    {
        while (cam.farClipPlane != view)
        {
            cam.farClipPlane = Mathf.Lerp(cam.farClipPlane, view, Time.deltaTime * 10f);
            yield return null;
        }

    }


    void FixedUpdate()
    {
        if (target != null)
            tr.position = Vector3.Lerp(tr.position, target.position + margin, Time.deltaTime * followSpeed);
    }
}
