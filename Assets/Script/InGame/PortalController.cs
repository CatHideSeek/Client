using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour {

    public bool isOpen = false;

    void Awake() {

    }

    public void Open() {
        if (!isOpen)
        {
            isOpen = true;
            gameObject.SetActive(true);
        }
            
    }

    public void Close() {
        if (isOpen) {
            isOpen = false;
            gameObject.SetActive(false);
        }
    }

}
