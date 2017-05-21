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
        }
            
    }

    public void Close() {
        if (isOpen) {
            isOpen = false;
        }
    }

}
