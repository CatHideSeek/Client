using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class IslandPos
{
	public int x, y;

	public IslandPos (int _x = 0, int _y = 0)
	{
		x = _x;
		y = _y;
	}

	public static IslandPos operator + (IslandPos pos1, IslandPos pos2)
	{
		return new IslandPos (pos1.x + pos2.x, pos1.y + pos2.y);
	}

	public static IslandPos operator - (IslandPos pos1, IslandPos pos2)
	{
		return new IslandPos (pos1.x - pos2.x, pos1.y - pos2.y);
	}
}

public class MapGenerator : MonoBehaviour
{
	public GameObject islandPrefab;
	public GameObject bridgePrefab;

	static int islandNum = 4;
	IslandPos currentPos = new IslandPos ();
	List<IslandPos> islandList = new List<IslandPos> ();

	public void InitMap ()
	{
		if (!PlayerDataManager.instance.my.isHost)
			return;
		
		for (int i = 0; i < islandNum; i++) {
			IslandPos temp;
			if (i == 0)
				temp = new IslandPos (0, 0);
			else
				temp = SetIsland (currentPos);
			if (i > 0) {
				GameObject obj = Instantiate (bridgePrefab, transform.position + new Vector3 ((temp.x + currentPos.x) / 2, 0, (temp.y + currentPos.y) / 2) * 10, Quaternion.identity);
				if (temp.y - currentPos.y != 0)
					obj.transform.Rotate (new Vector3 (0, 90, 0));
			} else
				currentPos = temp;
			GameObject islandGenerator = Instantiate (islandPrefab, transform.position + new Vector3 (temp.x, 0, temp.y) * 10, Quaternion.identity);
			islandList.Add (temp);
			currentPos = temp;
		}
	}

	IslandPos SetIsland (IslandPos pos)
	{
		IslandPos newPos;
		List<IslandPos> list = new List<IslandPos> ();
		list.Add (pos + new IslandPos (2, 0));
		list.Add (pos + new IslandPos (-2, 0));
		list.Add (pos + new IslandPos (0, 2));
		list.Add (pos + new IslandPos (0, -2));

		do {
			newPos = list [Random.Range (0, list.Count)];
		} while(CheckPos (newPos));

		return newPos;
	}

	bool CheckPos (IslandPos pos)
	{
		foreach (IslandPos iPos in islandList) {
			if (iPos.x == pos.x && iPos.y == pos.y) {
				return true;
			}
		}
		return false;
	}
}
