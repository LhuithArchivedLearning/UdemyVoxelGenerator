using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInteraction : MonoBehaviour
{

    public GameObject cam;
    public Block.BlockType currentBlockType;

    // Use this for initialization
    void Start()
    {
        currentBlockType = Block.BlockType.SAND;
    }

    // Update is called once per frame
    void Update()
    {   
        if(Input.GetKeyDown("1"))
            currentBlockType = Block.BlockType.SAND;
        if(Input.GetKeyDown("2"))
            currentBlockType = Block.BlockType.STONE;
        if(Input.GetKeyDown("3"))
            currentBlockType = Block.BlockType.DIAMOND;
        if(Input.GetKeyDown("4"))
            currentBlockType = Block.BlockType.REDSTONE;
        if(Input.GetKeyDown("5"))
            currentBlockType = Block.BlockType.GOLD;

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {

            RaycastHit hit;


            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 10))
            {
                Chunk hitc;

                if (!World.chunks.TryGetValue(hit.collider.gameObject.name, out hitc)) return;

                Vector3 hitBlock;
                if (Input.GetMouseButton(0))
                {
                    hitBlock = hit.point - hit.normal / 2.0f;
                }
                else
                {
                    hitBlock = hit.point + hit.normal / 2.0f;
                }

                Block b = World.GetWorldBlock(hitBlock);
   
                hitc = b.owner;

                bool update = false;

                if(Input.GetMouseButton(0))
                    update = b.HitBlock();
                else {
                     update = b.BuildBlock(currentBlockType);
                }
                

                if (update)
                {
                    hitc.changed = true;
                    List<string> updates = new List<string>();
                    float thisChunkx = hitc.chunk.transform.position.x;
                    float thisChunky = hitc.chunk.transform.position.y;
                    float thisChunkz = hitc.chunk.transform.position.z;

                    //updates.Add(hit.collider.gameObject.name);

                    //udpate neighbours?
                    if (b.position.x == 0)
                        updates.Add(World.BuildChunkName(new Vector3(thisChunkx - World.chunkSize, thisChunky, thisChunkz)));
                    if (b.position.x == World.chunkSize - 1)
                        updates.Add(World.BuildChunkName(new Vector3(thisChunkx + World.chunkSize, thisChunky, thisChunkz)));

                    if (b.position.y == 0)
                        updates.Add(World.BuildChunkName(new Vector3(thisChunkx, thisChunky - World.chunkSize, thisChunkz)));
                    if (b.position.y == World.chunkSize - 1)
                        updates.Add(World.BuildChunkName(new Vector3(thisChunkx, thisChunky + World.chunkSize, thisChunkz)));

                    if (b.position.z == 0)
                        updates.Add(World.BuildChunkName(new Vector3(thisChunkx, thisChunky, thisChunkz - World.chunkSize)));
                    if (b.position.z == World.chunkSize - 1)
                        updates.Add(World.BuildChunkName(new Vector3(thisChunkx, thisChunky, thisChunkz + World.chunkSize)));


                    foreach (string cname in updates)
                    {

                        Chunk c;

                        if (World.chunks.TryGetValue(cname, out c))
                        {
                            c.Redraw();
                        }
                    }
                }

            }
        }
    }
}
