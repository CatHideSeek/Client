using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public static CameraController instance;

    Transform tr;

    public Transform target;

    public Vector3 margin;
    public float followSpeed = 5f;

    public LayerMask opacityLayer;
    public RaycastHit[] hits;

    void Awake() {
        instance = this;
        tr = GetComponent<Transform>();
    }


    void FixedUpdate() {
        if (target != null)
        {
            tr.position = Vector3.Lerp(tr.position, target.position + margin, Time.deltaTime * followSpeed);
        }
    }
}
