using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour {

    public bool isOpen = false;

    public Transform hourObject, minObject, scrueObject;

    public GameObject scureParticle;

    public float hourSpeed = 100f, minSpeed = 400f, scrueSpeed = 200f;

    Animator ani;

    void Awake() {
        ani = transform.GetChild(0).GetComponent<Animator>();
    }

    private void Update()
    {
        if (isOpen)
            scrueObject.Rotate((Time.deltaTime) * scrueSpeed * Vector3.up);
        else
        {
            hourObject.Rotate((Time.deltaTime) * hourSpeed * Vector3.up);
            minObject.Rotate((Time.deltaTime) * minSpeed * Vector3.up);
        }
    }

    public void Open() {
        if (!isOpen)
        {
            isOpen = true;
            ani.SetTrigger("Open");
            gameObject.SetActive(true);
            scureParticle.SetActive(true);
        }
            
    }

    public void Close() {
        if (isOpen) {
            isOpen = false;
            ani.SetTrigger("Close");
            gameObject.SetActive(false);
            scureParticle.SetActive(false);
        }
    }

    public void CloseScrue()
    {
        if (isOpen)
        {
            isOpen = false;
            ani.SetTrigger("Close");
            scureParticle.SetActive(false);
        }
    }


}
