using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Material material;
    public Block[,,] chunkData;
    public GameObject chunk;
    public enum ChunkStatus{DRAW, DONE, KEEP};
    public ChunkStatus status;
    void BuildChunk()
    {

        chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];

        //Create Blocks
        for (int z = 0; z < World.chunkSize; z++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);

                    int worldX = (int)(x + chunk.transform.position.x);
                    int worldY = (int)(y + chunk.transform.position.y);
                    int worldZ = (int)(z + chunk.transform.position.z);

                    if (Utils.Utils.FBM3D(worldX, worldY, worldZ, 0.1f, 3) < 0.415f)
                        chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, chunk.gameObject, this);
                    else if (worldY <= Utils.Utils.GenerateStoneHeight(worldX, worldZ))
                    {
                        if (Utils.Utils.FBM3D(worldX, worldY, worldZ, 0.01f, 2) < 0.4f && worldY < 40)
                            chunkData[x, y, z] = new Block(Block.BlockType.DIAMOND, pos, chunk.gameObject, this);
                        else
                            chunkData[x, y, z] = new Block(Block.BlockType.STONE, pos, chunk.gameObject, this);
                    }

                    else if (worldY == Utils.Utils.GenerateHeight(worldX, worldZ))
                        chunkData[x, y, z] = new Block(Block.BlockType.GRASS, pos, chunk.gameObject, this);
                    else if (worldY <= Utils.Utils.GenerateHeight(worldX, worldZ))
                        chunkData[x, y, z] = new Block(Block.BlockType.DIRT, pos, chunk.gameObject, this);
                    else
                        chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, chunk.gameObject, this);

                    status = ChunkStatus.DRAW;
                }
    }

    public void DrawChunk()
    {
        //Draw Blocks
        for (int z = 0; z < World.chunkSize; z++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int x = 0; x < World.chunkSize; x++)
                {
                    chunkData[x, y, z].Draw();
                }

        CombineQuads();
        MeshCollider collider = chunk.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        collider.sharedMesh = chunk.transform.GetComponent<MeshFilter>().mesh;
    }

    void CombineQuads()
    {
        //1. Combine all children meshes
        MeshFilter[] meshFilters = chunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;

        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        //2. Create a new mesh on the parent Object
        MeshFilter mf = (MeshFilter)chunk.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();

        //3. Add combined meshes on children as the parents mesh
        mf.mesh.CombineMeshes(combine);

        //4. Create renderer for Parent
        MeshRenderer renderer = chunk.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.material = material;

        foreach (Transform quad in chunk.transform)
        {
            GameObject.Destroy(quad.gameObject);
        }
    }

    public Chunk(Vector3 position, Material c)
    {
        chunk = new GameObject(World.BuildChunkName(position));
        chunk.transform.position = position;
        material = c;
        BuildChunk();
    }
}
