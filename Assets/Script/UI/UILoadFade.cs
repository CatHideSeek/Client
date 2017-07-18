using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoadFade : MonoBehaviour {
    public static UILoadFade instance;
    public Text text;
    public bool fading = false;
    float cnt = 0;
    Image image;
    float fa = 1;

	// Use this for initialization
	void Start () {
        instance = this;
        image = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        if(fading)
        {
            fa -= 2 * Time.deltaTime;
            image.color=new Color(image.color.r,image.color.g,image.color.b,fa);
            text.color = new Color(text.color.r, text.color.g, text.color.b, fa);
            if(fa<=0)
                gameObject.SetActive(false);
        }
        else
        {
            cnt += 2 * Time.deltaTime;
            if (cnt > 5)
                cnt = 0;

            text.text = "loading";
            for (int i = 0; i <= cnt; i++)
            {
                text.text += ".";
            }
        }
	}
}
