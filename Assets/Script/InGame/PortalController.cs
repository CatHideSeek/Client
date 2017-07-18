using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour {

    public bool isOpen = false;

    public Transform hourObject, minObject, scrueObject;

    Animator ani;

    void Awake() {
        ani = transform.GetChild(0).GetComponent<Animator>();
    }

    private void Update()
    {
        if (isOpen)
            scrueObject.Rotate((Time.deltaTime) * 400f * Vector3.up);
        else
        {
            hourObject.Rotate((Time.deltaTime) * 100f * Vector3.up);
            minObject.Rotate((Time.deltaTime) * 400f * Vector3.up);
        }
    }

    public void Open() {
        if (!isOpen)
        {
            isOpen = true;
            ani.SetTrigger("Open");
            gameObject.SetActive(true);
        }
            
    }

    public void Close() {
        if (isOpen) {
            isOpen = false;
            ani.SetTrigger("Close");
            gameObject.SetActive(false);
        }
    }



}
