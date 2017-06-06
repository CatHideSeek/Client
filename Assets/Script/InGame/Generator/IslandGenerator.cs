using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class BlockPos
{
	public int x, y;

	public BlockPos (int _x = 0, int _y = 0)
	{
		x = _x;
		y = _y;
	}

	public Vector3 ToVector3 (int h)
	{
		return new Vector3 (x - 4, h, y - 4);
	}

	public static bool operator == (BlockPos pos1, BlockPos pos2)
	{
		if (System.Object.ReferenceEquals (pos1, pos2)) {
			return true;
		}

		if (((object)pos1 == null) || ((object)pos2 == null)) {
			return false;
		}


		return pos1.x == pos2.x && pos1.y == pos2.y;
	}

	public static bool operator != (BlockPos pos1, BlockPos pos2)
	{
		return !(pos1 == pos2);
	}
}

public class IslandGenerator : MonoBehaviour
{
	public static int maxFloor = 6;
	public static int minTreeNum = 0;
	public static int maxTreeNum = 3;
	public static int minBushNum = 3;
	public static int maxBushNum = 6;

	public GameObject treePrefab;
	public GameObject bushPrefab;

	List<BlockPos>[] mapList = new List<BlockPos>[maxFloor];
	List<BlockPos>[] topMapList = new List<BlockPos>[maxFloor];
	//Leave top blocks only

	void Awake ()
	{
		//Init 'mapList' and 'topMapList', Call 'SetBlocks()'
		for (int i = 0; i < maxFloor; i++) {
			mapList [i] = new List<BlockPos> ();
			topMapList [i] = new List<BlockPos> ();
			SetBlocks (i, 6 - i, 6 - i);
		}

		//Create blocks
		for (int h = 0; h < maxFloor; h++) {
			foreach (BlockPos pos in mapList[h]) {
				if (transform.position + pos.ToVector3 (0) == Vector3.zero)
					GameManager.instance.spawnPos.y = h+2;
				Vector3 blockPos = transform.position + pos.ToVector3 (h);
				int blockId = Random.Range (0, GameManager.instance.blockObject.Length);

				Instantiate (GameManager.instance.blockObject[blockId], blockPos, Quaternion.Euler(-90,0,0));
				GameManager.instance.blockList.Add (new Block (blockPos, blockId));
			}
		}

		CreateTrees (Random.Range (minTreeNum, maxTreeNum + 1));
		CreateBushes (Random.Range (minBushNum, maxBushNum + 1));
	}

	void SetBlocks (int floor, int min, int max)
	{
		if (floor > 0) {
			for (int i = 0; i < Random.Range (floor, floor * 2 - (1 + floor / 2)); i++) {
				//Set size with random
				int size = Random.Range (min, max);
				size += size - 1;

				//Choose position with random in 'mapList'
				BlockPos pos = mapList [floor - 1] [Random.Range (0, mapList [floor - 1].Count)];
				if (floor == 1) {
					pos = new BlockPos (4, 4);
					size = 7;
				}

				//Set blocks
				for (int y = pos.y - size / 2; y <= pos.y + size / 2; y++) {
					for (int x = pos.x - size / 2; x <= pos.x + size / 2; x++) {
						BlockPos blockPos = new BlockPos (x, y);

						//Remove overlap blocks in 'topMapList'
						for (int j = 0; j <= floor; j++) {
							List<BlockPos> overlaps = topMapList [j].FindAll (delegate(BlockPos iter) {
								return iter == blockPos;
							});
							foreach (BlockPos overlap in overlaps) {
								if (overlap != null) {
									topMapList [j].Remove (overlap);
									if (j == floor) {
										mapList [j].Remove (overlap);
									}
								}
							}

						}

						mapList [floor].Add (blockPos);
						topMapList [floor].Add (blockPos);
					}
				}
			}
		} else {
			//Set blocks
			for (int y = 0; y < 9; y++) {
				for (int x = 0; x < 9; x++) {
					mapList [floor].Add (new BlockPos (x, y));
					topMapList [floor].Add (new BlockPos (x, y));
				}
			}
		}
	}

	void CreateTrees (int cnt)
	{
		//Create trees
		for (int i = 0; i < cnt; i++) {
			int h = Random.Range (4, 6);
			if (topMapList [h].Count > 0) {
				int r = Random.Range (0, topMapList [h].Count);
				Instantiate (treePrefab, transform.position + topMapList [h] [r].ToVector3 (h + 1), Quaternion.identity);
				topMapList [h].Remove (topMapList [h] [r]);//이미 생성된 곳은 중복을 방지하기 위해서 리스트에서 지워준다
			}
		}
	}

	void CreateBushes (int cnt)
	{
		//Create bushes
		for (int i = 0; i < cnt; i++) {
			int h = Random.Range (0, 6);
			if (topMapList [h].Count > 0) {
				int r = Random.Range (0, topMapList [h].Count);
				Instantiate (bushPrefab, transform.position + topMapList [h] [r].ToVector3 (h + 1), Quaternion.identity);
				topMapList [h].Remove (topMapList [h] [r]);//이미 생성된 곳은 중복을 방지하기 위해서 리스트에서 지워준다
			}
		}
	}
}