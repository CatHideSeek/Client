using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpacityObject : MonoBehaviour {
    [SerializeField]
    float opaSpeed = 2f;

    bool opacity=false;
    Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Color c = mat.color;
        if(opacity)
        {
            c.a = Mathf.Lerp(c.a, 0.5f,Time.deltaTime * opaSpeed);
            mat.color = c;
            if (!checkOpacity())
                opacity = false;
        }
        else if(c.a<1)
        {
            c.a = Mathf.Lerp(c.a, 1f, Time.deltaTime * opaSpeed);
            mat.color = c;
        }
	}

    protected virtual bool checkOpacity()
    {
        foreach(RaycastHit hit in Camera.main.GetComponent<CameraController>().hits)
        {
            if (hit.transform.gameObject.Equals(gameObject))
                return true;
        }
        return false;
    }

    public void SetOpacity(bool _opacity=true)
    {
        opacity = _opacity;
    }
}