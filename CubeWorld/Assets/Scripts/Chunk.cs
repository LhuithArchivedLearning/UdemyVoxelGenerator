using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    public Material material;
    public Block[,,] chunkData;
    IEnumerator BuildChunk(int sizeX, int sizeY, int sizeZ)
    {

        chunkData = new Block[sizeX, sizeY, sizeZ];

        //Create Blocks
        for (int z = 0; z < sizeZ; z++)
            for (int y = 0; y < sizeY; y++)
                for (int x = 0; x < sizeX; x++)
                {

                    Vector3 pos = new Vector3(x, y, z);
                    if (Random.RandomRange(0, 100) < 50)
                    {
                        chunkData[x, y, z] = new Block(Block.BlockType.DIRT, pos,
                                                this.gameObject, material);
                    }
                    else
                    {
                        chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos,
                    this.gameObject, material);
                    }

                }

        //Draw Blocks
        for (int z = 0; z < sizeZ; z++)
            for (int y = 0; y < sizeY; y++)
                for (int x = 0; x < sizeX; x++)
                {
                    chunkData[x, y, z].Draw();

                }



        CombineQuads();
		                    yield return null;
    }
    // Use this for initialization
    void Start()
    {
        StartCoroutine(BuildChunk(32, 32, 32));
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CombineQuads()
    {

        //1. Combine all children meshes
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;

        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        //2. Create a new mesh on the parent Object
        MeshFilter mf = (MeshFilter)this.gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();

        //3. Add combined meshes on children as the parents mesh
        mf.mesh.CombineMeshes(combine);

        //4. Create renderer for Parent
        MeshRenderer renderer = this.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.material = material;

        foreach (Transform quad in this.transform)
        {
            Destroy(quad.gameObject);
        }
    }
}
