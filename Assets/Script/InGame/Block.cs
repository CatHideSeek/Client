using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
	public Vector3 pos;
	public int id;

	public Block(Vector3 _pos,int _id)
	{
		pos = _pos;
		id = _id;
	}
}