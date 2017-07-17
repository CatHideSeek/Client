using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateAlarm : MonoBehaviour {
	public bool goRight;
	public float speed;
	public Text text;

	public void PushAlarm(string str)
	{
		text.text = str;
	}
}
