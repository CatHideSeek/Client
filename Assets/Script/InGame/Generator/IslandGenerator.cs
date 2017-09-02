using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class BlockPos
{
    public int x, y;

    public BlockPos(int _x = 0, int _y = 0)
    {
        x = _x;
        y = _y;
    }

    public Vector3 ToVector3(int h)
    {
		return new Vector3(x - IslandGenerator.instance.islandSize/2, h, y - IslandGenerator.instance.islandSize/2);
    }

    public static bool operator ==(BlockPos pos1, BlockPos pos2)
    {
        if (System.Object.ReferenceEquals(pos1, pos2))
        {
            return true;
        }

        if (((object)pos1 == null) || ((object)pos2 == null))
        {
            return false;
        }


        return pos1.x == pos2.x && pos1.y == pos2.y;
    }

    public static bool operator !=(BlockPos pos1, BlockPos pos2)
    {
        return !(pos1 == pos2);
    }
}

public class IslandGenerator : MonoBehaviour
{
	public static IslandGenerator instance;
    public static int maxFloor = 4;
    public int minTreeNum = 3;
    public int maxTreeNum = 6;
    public int minBushNum = 3;
    public int maxBushNum = 6;
    public int minKRockNum = 3;
    public int maxRockNum = 6;
    public int islandSize=15;
    public int keyNum=1;
    public int itemNum = 1;
    public int id = -1;

    List<BlockPos>[] mapList = new List<BlockPos>[maxFloor];
    List<BlockPos>[] topMapList = new List<BlockPos>[maxFloor];
    //Leave top blocks only


    public List<GameObject> blocks = new List<GameObject>();
    public bool useMipMaps = true;
    public TextureFormat textureFormat = TextureFormat.RGB24;

    public void Init()
    {
        instance = this;

        if (!PlayerDataManager.instance.my.isHost)
            return;

        //Init 'mapList' and 'topMapList', Call 'SetBlocks()'
        for (int i = 0; i < maxFloor; i++)
        {
            mapList[i] = new List<BlockPos>();
            topMapList[i] = new List<BlockPos>();
            SetBlocks(i);
        }

        //Create blocks
        for (int h = 0; h < maxFloor; h++)
        {
            foreach (BlockPos pos in mapList[h])
            {
                if (transform.position + pos.ToVector3(0) == Vector3.zero)
                    GameManager.instance.spawnPos.y = h + 2;
                Vector3 blockPos = transform.position + pos.ToVector3(h);
                int blockId = Random.Range(0, 4);

                GameObject g = Instantiate(GameManager.instance.blockObject[blockId], blockPos, Quaternion.Euler(-90, 0, 0));
                g.transform.parent = transform;
                blocks.Add(g);
                GameManager.instance.blockList.Add(new Block(blockPos, blockId, id));
            }
        }
        

		if (GameManager.instance.portalIsland == id)
			CreatePortal ();

        CreateTrees(Random.Range(minTreeNum, maxTreeNum + 1));
        CreateBushes(Random.Range(minBushNum, maxBushNum + 1));
        CreateRocks(Random.Range(minKRockNum, maxRockNum + 1));
        CreateKeys(keyNum);
        CreateItems(itemNum);
    }

    void SetBlocks(int floor)
    {
        if (floor > 0)
        {
			for (int i = 0; i < floor; i++)
            {
                //Set size with random
				int size = islandSize-floor*(islandSize/(maxFloor-1));

                //Choose position with random in 'mapList'
				BlockPos pos;
				do{
				pos = mapList[floor - 1][Random.Range(0, mapList[floor - 1].Count)];
				}while(pos.y<islandSize/2||pos.x>islandSize/2);
                if (floor == 1)
                {
					pos = new BlockPos(islandSize/2-2, islandSize/2+2);
					size = islandSize;
                }

                //Set blocks
                for (int y = pos.y - size / 2; y <= pos.y + size / 2; y++)
                {
                    for (int x = pos.x - size / 2; x <= pos.x + size / 2; x++)
                    {
                        BlockPos blockPos = new BlockPos(x, y);

                        //Remove overlap blocks in 'topMapList'
                        for (int j = 0; j <= floor; j++)
                        {
                            List<BlockPos> overlaps = topMapList[j].FindAll(delegate (BlockPos iter){return iter == blockPos;});
                            foreach (BlockPos overlap in overlaps)
                            {
                                if (overlap != null)
                                {
                                    topMapList[j].Remove(overlap);
                                    if (j == floor)
                                    {
                                        mapList[j].Remove(overlap);
                                    }
                                }
                            }

                        }

                        mapList[floor].Add(blockPos);
                        topMapList[floor].Add(blockPos);
                    }
                }
            }
        }
        else
        {
            //Set blocks
			for (int y = 0; y < islandSize; y++)
            {
				for (int x = 0; x < islandSize; x++)
                {
                    mapList[floor].Add(new BlockPos(x, y));
                    topMapList[floor].Add(new BlockPos(x, y));
                }
            }
        }
    }

    private void CreateTrees(int cnt)
    {
        //Create trees
        for (int i = 0; i < cnt; i++)
        {
			int h = Random.Range(0, maxFloor);
            if (topMapList[h].Count > 0)
            {
                int r = Random.Range(0, topMapList[h].Count);
                GameObject g = Instantiate(GameManager.instance.blockObject[4], transform.position + topMapList[h][r].ToVector3(h + 1), Quaternion.Euler(-90, 0, 0));
                g.transform.parent = transform;
                topMapList[h].Remove(topMapList[h][r]);//이미 생성된 곳은 중복을 방지하기 위해서 리스트에서 지워준다
                GameManager.instance.blockList.Add(new Block(g.transform.position, 4,id));
            }
        }
    }

    private void CreateBushes(int cnt)
    {
        //Create bushes
        for (int i = 0; i < cnt; i++)
        {
			int h = Random.Range(0, maxFloor);
            if (topMapList[h].Count > 0)
            {
                int r = Random.Range(0, topMapList[h].Count);
                GameObject g = Instantiate(GameManager.instance.blockObject[5], transform.position + topMapList[h][r].ToVector3(h + 1), Quaternion.Euler(-90, 0, 0));
                g.transform.parent = transform;
                topMapList[h].Remove(topMapList[h][r]);//이미 생성된 곳은 중복을 방지하기 위해서 리스트에서 지워준다
                GameManager.instance.blockList.Add(new Block(g.transform.position, 5,id));
            }
        }
    }

    private void CreateRocks(int cnt)
    {
        //Create bushes
        for (int i = 0; i < cnt; i++)
        {
            int h = Random.Range(0, maxFloor);
            if (topMapList[h].Count > 0)
            {
                int r = Random.Range(0, topMapList[h].Count);
                GameObject g = Instantiate(GameManager.instance.blockObject[10], transform.position + topMapList[h][r].ToVector3(h + 1), Quaternion.Euler(-90, 0, 0));
                g.transform.parent = transform;
                topMapList[h].Remove(topMapList[h][r]);//이미 생성된 곳은 중복을 방지하기 위해서 리스트에서 지워준다
                GameManager.instance.blockList.Add(new Block(g.transform.position, 10,id));
            }
        }
    }

    private void CreateKeys(int cnt)
    {
        for (int i = 0; i < cnt; i++)
        {
            int h = Random.Range(0, maxFloor);
            if (topMapList[h].Count > 0)
            {
                int r = Random.Range(0, topMapList[h].Count);
                GameObject g = Instantiate(GameManager.instance.blockObject[8], transform.position + topMapList[h][r].ToVector3(h + 1), Quaternion.Euler(-90, 0, 0));
                g.transform.parent = transform;
                g.name += "(" + i + ")";
                topMapList[h].Remove(topMapList[h][r]);//이미 생성된 곳은 중복을 방지하기 위해서 리스트에서 지워준다
                GameManager.instance.blockList.Add(new Block(g.transform.position, 8,id));
            }
        }
    }

    private void CreateItems(int cnt)
    {
        for (int i = 0; i < cnt; i++)
        {
            int h = Random.Range(0, maxFloor);
            if (topMapList[h].Count > 0)
            {
                int r = Random.Range(0, topMapList[h].Count);
                GameObject g = Instantiate(GameManager.instance.blockObject[11], transform.position + topMapList[h][r].ToVector3(h + 1), Quaternion.Euler(-90, 0, 0));
                g.transform.parent = transform;
                g.GetComponent<Spawner>().s = id%3;
                g.name += "(" + i+")";
                topMapList[h].Remove(topMapList[h][r]);//이미 생성된 곳은 중복을 방지하기 위해서 리스트에서 지워준다
                GameManager.instance.blockList.Add(new Block(g.transform.position, 11,id));
            }
        }
    }

	private void CreatePortal()
	{
		int h = Random.Range(0, maxFloor);
		if (topMapList[h].Count > 0)
		{
			int r = topMapList [h].Count - 1;
			GameObject g = Instantiate(GameManager.instance.blockObject[12], transform.position + topMapList[h][r].ToVector3(h + 2), Quaternion.Euler(0, 45, 0));
			g.transform.parent = transform;
            GameManager.instance.portalObject = g;
            GameManager.instance.portal = g.GetComponent<PortalController>();
			topMapList[h].Remove(topMapList[h][r]);//이미 생성된 곳은 중복을 방지하기 위해서 리스트에서 지워준다
			GameManager.instance.blockList.Add(new Block(g.transform.position, 12,id));

			GameManager.instance.portalObject = g;
		}
	}

}