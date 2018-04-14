using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
class BlockData{

    public Block.BlockType[,,] matrix;
    public BlockData(){}

    public BlockData(Block[,,] b){
        matrix = new Block.BlockType[World.chunkSize, World.chunkSize,World.chunkSize];
        for(int z = 0; z < World.chunkSize; z++)
            for(int y = 0; y < World.chunkSize; y++)
                for(int x = 0; x < World.chunkSize; x++){
                    matrix[x,y,z] = b[x,y,z].bType;
                }
    }
}

public class Chunk
{
    public Material material;
    public Block[,,] chunkData;
    public GameObject chunk;
    public enum ChunkStatus{DRAW, DONE, KEEP};
    public ChunkStatus status;
    BlockData bd;

	string BuildChunkFileName(Vector3 v)
	{
		return Application.persistentDataPath + "/savedata/Chunk_" + 
								(int)v.x + "_" +
									(int)v.y + "_" +
										(int)v.z + 
										"_" + World.chunkSize +
										"_" + World.radius +
										".dat";
	}

	bool Load() //read data from file
	{
		string chunkFile = BuildChunkFileName(chunk.transform.position);
		if(File.Exists(chunkFile))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(chunkFile, FileMode.Open);
			bd = new BlockData();
			bd = (BlockData) bf.Deserialize(file);
			file.Close();
			//Debug.Log("Loading chunk from file: " + chunkFile);
			return true;
		}
		return false;
	}

    public void Redraw()
    {
        GameObject.DestroyImmediate(chunk.GetComponent<MeshFilter>());
        GameObject.DestroyImmediate(chunk.GetComponent<MeshRenderer>());
        GameObject.DestroyImmediate(chunk.GetComponent<Collider>());
        DrawChunk();
    }

	public void Save() //write data to file
	{
		string chunkFile = BuildChunkFileName(chunk.transform.position);
		
		if(!File.Exists(chunkFile))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(chunkFile));
		}
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(chunkFile, FileMode.OpenOrCreate);
		bd = new BlockData(chunkData);
		bf.Serialize(file, bd);
		file.Close();
		//Debug.Log("Saving chunk from file: " + chunkFile);
	}

    void BuildChunk()
    {

        chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];
        bool dataFromFile = false;
		dataFromFile = Load();

        //Create Blocks
        for (int z = 0; z < World.chunkSize; z++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);

                    int worldX = (int)(x + chunk.transform.position.x);
                    int worldY = (int)(y + chunk.transform.position.y);
                    int worldZ = (int)(z + chunk.transform.position.z);

                    if(dataFromFile)
					{
						chunkData[x,y,z] = new Block(bd.matrix[x, y, z], pos, 
						                chunk.gameObject, this);
						continue;
					}


					int surfaceHeight = Utils.Utils.GenerateHeight(worldX,worldZ);

                    if (Utils.Utils.FBM3D(worldX, worldY, worldZ, 0.1f, 3) < 0.415f)
                        chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, chunk.gameObject, this);
                    else if (worldY <= Utils.Utils.GenerateStoneHeight(worldX, worldZ))
                    {
                        if (Utils.Utils.FBM3D(worldX, worldY, worldZ, 0.001f, 3) < 0.4f && worldY < 40)
                            chunkData[x, y, z] = new Block(Block.BlockType.DIAMOND, pos, chunk.gameObject, this);
                        else
                            chunkData[x, y, z] = new Block(Block.BlockType.STONE, pos, chunk.gameObject, this);
                    }

                    else if (worldY == surfaceHeight)
                        chunkData[x, y, z] = new Block(Block.BlockType.GRASS, pos, chunk.gameObject, this);
                    else if (worldY <= surfaceHeight)
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
        status = ChunkStatus.DONE;
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
    public Chunk(){}
    
    public Chunk(Vector3 position, Material c)
    {
        chunk = new GameObject(World.BuildChunkName(position));
        chunk.transform.position = position;
        material = c;
        BuildChunk();
    }
}
