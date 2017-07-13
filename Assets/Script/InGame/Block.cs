using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
	public Vector3 pos;
	public int id;
    public int parent;

	public Block(Vector3 _pos,int _id,int _parent=-1)
	{
		pos = _pos;
		id = _id;
        parent = _parent;
	}
}