using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class IslandPos
{
	public int x, y,type;

	public IslandPos (int _x = 0, int _y = 0, int _type=0)
	{
		x = _x;
		y = _y;
		type=_type;
	}

	public static IslandPos operator + (IslandPos pos1, IslandPos pos2)
	{
		return new IslandPos (pos1.x + pos2.x, pos1.y + pos2.y,pos2.type);
	}

	public static IslandPos operator - (IslandPos pos1, IslandPos pos2)
	{
		return new IslandPos (pos1.x - pos2.x, pos1.y - pos2.y,pos2.type);
	}
}

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator instance;
	public GameObject islandPrefab;
	public GameObject bridgePrefab;
	public int bridgeGab=10;
	public int islandNum;

	IslandPos currentPos = new IslandPos ();

    [SerializeField]
	List<IslandPos> islandList = new List<IslandPos> ();

    public bool useMipMaps = true;
    public TextureFormat textureFormat = TextureFormat.RGB24;

    void Awake()
    {
        instance = this;

    }

	void Start()
	{
		GameManager.instance.portalIsland = Random.Range (0, MapGenerator.instance.islandNum);
	}

    public void InitMap ()
	{
		if (!PlayerDataManager.instance.my.isHost)
			return;


        Debug.Log("InitMap()");

        for (int i = 0; i < islandNum; i++) {
			IslandPos temp;
			if (i == 0)
				temp = new IslandPos (0, 0);
			else
				temp = SetIsland (currentPos);
			if (i > 0) {
				Vector3 bridgePos = transform.position + new Vector3 ((temp.x + currentPos.x) / 2, 0, (temp.y + currentPos.y) / 2) * bridgeGab ;
				if (temp.type < 2) {
					bridgePos.z -= IslandGenerator.instance.islandSize/2-1;
				} else {
					bridgePos.x += IslandGenerator.instance.islandSize/2-1;
				}
				GameObject obj = Instantiate (bridgePrefab, bridgePos, Quaternion.identity);
                if (temp.y - currentPos.y != 0)
                {
                    obj.transform.Rotate(new Vector3(0, 90, 0));
                    GameManager.instance.blockList.Add(new Block(obj.transform.position, 6));
                }
                else
                    GameManager.instance.blockList.Add(new Block(obj.transform.position, 7));
                obj.transform.parent = transform;
                obj.name +=  " "+(i-1)+">>"+i;
                CombineBridge(obj);

			} else
				currentPos = temp;

            CreateIsland(temp.x, temp.y, i);
            islandList.Add(temp);
            GameManager.instance.islandList.Add(new IslandInfo(temp.x, temp.y, i));
			currentPos = temp;
		}

        transform.Rotate(new Vector3(0, 45, 0));
	}

    public void CreateIsland(int x,int y,int id)
    {
        GameObject islandGenerator = Instantiate(islandPrefab, transform.position + new Vector3(x, 0,y) * bridgeGab, Quaternion.identity);
        islandGenerator.transform.parent = transform;
        islandGenerator.name = "Island " + id;
        islandGenerator.GetComponent<IslandGenerator>().id = id;
        islandGenerator.GetComponent<IslandGenerator>().Init();
    }

	IslandPos SetIsland (IslandPos pos)
	{
		IslandPos newPos;
		List<IslandPos> list = new List<IslandPos> ();
		list.Add (pos + new IslandPos (2, 0,0));
		list.Add (pos + new IslandPos (-2, 0,1));
		list.Add (pos + new IslandPos (0, 2, 2));
		list.Add (pos + new IslandPos (0, -2,3));

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

    private void CombineBridge(GameObject obj)
    {

        int size;
        int originalSize;
        int pow2;
        Texture2D combinedTexture;
        Material material;
        Texture2D texture;
        Mesh mesh;
        Hashtable textureAtlas = new Hashtable();

        List<GameObject> blocks = new List<GameObject>();

        //블럭을 로드
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            blocks.Add(obj.transform.GetChild(i).gameObject);
        }


        if (blocks.Count > 1)
        {
            originalSize = blocks[0].GetComponent<MeshRenderer>().material.mainTexture.width;
            //pow2 = GetTextureSize(blocks);
            pow2 = 8;
            size = pow2 * originalSize;
            combinedTexture = new Texture2D(size, size, textureFormat, useMipMaps);

            for (int i = 0; i < blocks.Count; i++)
            {
                texture = (Texture2D)blocks[i].GetComponent<MeshRenderer>().material.mainTexture;
                if (!textureAtlas.ContainsKey(texture))
                {

                    combinedTexture.SetPixels((int)((i % pow2) * originalSize), (int)((i / pow2) * originalSize), originalSize, originalSize, texture.GetPixels());
                    textureAtlas.Add(texture, new Vector2(i % pow2, i / pow2));
                }
            }
            combinedTexture.Apply();

            material = new Material(Shader.Find("Standard"));
            material.mainTexture = combinedTexture;

            for (int i = 0; i < blocks.Count; i++)
            {
                mesh = blocks[i].GetComponent<MeshFilter>().mesh;
                Vector2[] uv = new Vector2[mesh.uv.Length];
                Vector2 offset;
                if (textureAtlas.ContainsKey(blocks[i].GetComponent<MeshRenderer>().material.mainTexture))
                {
                    offset = (Vector2)textureAtlas[blocks[i].GetComponent<MeshRenderer>().material.mainTexture];
                    for (int u = 0; u < mesh.uv.Length; u++)
                    {
                        uv[u] = mesh.uv[u] / (float)pow2;
                        uv[u].x += ((float)offset.x) / (float)pow2;
                        uv[u].y += ((float)offset.y) / (float)pow2;
                    }
                }
                else
                {
                    //에러 예외
                }

                mesh.uv = uv;
                blocks[i].GetComponent<MeshRenderer>().material = material;
            }


            int staticCount = 0;
            CombineInstance[] combine = new CombineInstance[blocks.Count];
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].isStatic)
                {
                    staticCount++;
                    combine[i].mesh = blocks[i].GetComponent<MeshFilter>().mesh;
                    combine[i].transform = blocks[i].transform.localToWorldMatrix;
                }
            }

            if (staticCount > 1)
            {
                GameObject g = new GameObject("BridgeRender");
                MeshFilter filter = g.AddComponent<MeshFilter>();
                MeshRenderer renderer = g.AddComponent<MeshRenderer>();
                filter.mesh = new Mesh();
                filter.mesh.CombineMeshes(combine);
                renderer.material = material;

                //생성된 블럭을 삭제합니다.
                for (int i = 0; i < blocks.Count; i++)
                {
                    if (blocks[i].isStatic)
                    {
                        Destroy(blocks[i]);
                    }
                }
                g.transform.parent = obj.transform;
            }

            Resources.UnloadUnusedAssets();
        }
    }


    private int GetTextureSize(List<GameObject> o)
    {
        List<Texture> textures = new List<Texture>();
        // 텍스쳐를 검색
        for (int i = 0; i < o.Count; i++)
        {
            if (!textures.Contains(o[i].GetComponent<MeshRenderer>().material.mainTexture))
            {
                textures.Add(o[i].GetComponent<MeshRenderer>().material.mainTexture);
            }
        }
        if (textures.Count == 1) return 1;
        if (textures.Count < 5) return 2;
        if (textures.Count < 17) return 4;
        if (textures.Count < 65) return 8;
        //아무것도 없는 경우
        return 0;
    }

}
