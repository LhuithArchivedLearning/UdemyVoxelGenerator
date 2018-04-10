using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Realtime.Messaging.Internal;

public class World : MonoBehaviour
{
    public GameObject player;
    public Material textureAtlas;
    public static int ColumnHeight = 16;
    public static int chunkSize = 16;
    public static int worldSize = 1;
    public int WorldSize;
    public static int radius = 4;
    public static bool isRandom;
    public bool IsRandom;
    // Use this for initialization
    public static ConcurrentDictionary<string, Chunk> chunks;
    //public Slider loadingAmount;
    //public Text loadingTextValue;
    //public Camera camera;
    //public Button playButton;

    public static  bool firstbuild = true;
    bool building = false;

    public static string BuildChunkName(Vector3 v)
    {
        return (int)v.x + "_" +
                (int)v.y + "_" +
                (int)v.z;
    }

    void BuildChunkAt(int x, int y, int z){
        Vector3 chunkPosition = new Vector3(x * chunkSize,
                                            y * chunkSize,
                                            z * chunkSize);
        string n = BuildChunkName(chunkPosition);
        Chunk c;

        if(!chunks.TryGetValue(n, out c)){
            c = new Chunk(chunkPosition, textureAtlas);
            c.chunk.transform.parent = this.transform;
            chunks.TryAdd(c.chunk.name, c);
        }
    }

    IEnumerator BuildRecursiveWorld(int x, int y, int z, int rad){

        yield return null;
    }

    IEnumerator DrawChunks(){
        foreach(KeyValuePair<string, Chunk> c in chunks){
            if(c.Value.status == Chunk.ChunkStatus.DRAW){
                c.Value.DrawChunk();
            }

            yield return null;
        }
    }
    
   //IEnumerator BuildChunkColumn()
   //{
   //    for (int i = 0; i < ColumnHeight; i++)
   //    {

   //        Vector3 chunkPosition = new Vector3(this.transform.position.x, i * chunkSize, this.transform.position.z);
   //        Chunk c = new Chunk(chunkPosition, textureAtlas);
   //        c.chunk.transform.parent = this.transform;
   //        chunks.Add(c.chunk.name, c);
   //    }

   //    foreach (KeyValuePair<string, Chunk> c in chunks)
   //    {
   //        c.Value.DrawChunk();
   //        yield return null;
   //    }
   //}

   //Enumerator BuildWorld()
   //
   //   building = true;
   //   int posx = (int)Mathf.Floor(player.transform.position.x / chunkSize);
   //   int posz = (int)Mathf.Floor(player.transform.position.z / chunkSize);

   //   float totalChunks = (Mathf.Pow((radius * 2) + 1, 2) * ColumnHeight) * 2;
   //   int processCount = 0;

   //   for (int z = -radius; z <= radius; z++)
   //       for (int x = -radius; x <= radius; x++)
   //           for (int y = 0; y < ColumnHeight; y++)
   //           {
   //               Vector3 chunkPosition = new Vector3((x + posx) * chunkSize, y * chunkSize, (posz + z) * chunkSize);

   //               Chunk c;
   //               string n = BuildChunkName(chunkPosition);

   //               if (chunks.TryGetValue(n, out c))
   //               {
   //                   c.status = Chunk.ChunkStatus.KEEP;
   //                   break;
   //               }
   //               else
   //               {
   //                   c = new Chunk(chunkPosition, textureAtlas);
   //                   c.chunk.transform.parent = this.transform;
   //                   chunks.Add(c.chunk.name, c);
   //               }

   //               if (firstbuild)
   //               {
   //                  // processCount++;
   //                  // loadingAmount.value = processCount / totalChunks * 100;
   //                  // loadingTextValue.text = "Loading..." + Mathf.FloorToInt(processCount / totalChunks * 100) + "%";
   //               }


   //               yield return null;
   //           }

   //   foreach (KeyValuePair<string, Chunk> c in chunks)
   //   {

   //       if (c.Value.status == Chunk.ChunkStatus.DRAW)
   //       {
   //           c.Value.DrawChunk();
   //           c.Value.status = Chunk.ChunkStatus.KEEP;
   //       }

   //       //Delete Old Chunks

   //       c.Value.status = Chunk.ChunkStatus.DONE;

   //       if (firstbuild)
   //       {
   //          // processCount++;
   //          // loadingAmount.value = processCount / totalChunks * 100;
   //          // if (Mathf.FloorToInt(processCount / totalChunks * 100) < 100)
   //          //     loadingTextValue.text = "Loading..." + Mathf.FloorToInt(processCount / totalChunks * 100) + "%";
   //          // else
   //          //     loadingTextValue.text = "READY!";
   //       }
   //       yield return null;
   //   }
//
//
//     if (firstbuild)
//     {
//         player.SetActive(true);
//         //playButton.gameObject.SetActive(false);
//        // loadingAmount.gameObject.SetActive(false);
//         //camera.gameObject.SetActive(false);
//         //loadingTextValue.gameObject.SetActive(false);
//         firstbuild = false;
//     }
//
//     building = false;
//
// }

    public void StartBuild()
    {
      //  StartCoroutine(BuildWorld());
    }
    void Start()
    {
        player.SetActive(false);
        chunks = new ConcurrentDictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        if(!building && !firstbuild){
            StartCoroutine(BuildWorld());
        }
    }

    void OnValidate()
    {
        worldSize = WorldSize;
        isRandom = IsRandom;
    }
}
