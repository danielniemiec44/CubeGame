using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MeshUpdater : MonoBehaviour
{
    public static int playerChunkX;
    public static int playerChunkZ;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateMeshAsync(MeshInstance instance) {
        StartCoroutine(updateMesh(instance));
    }

    public void updateViewAsync() {
        StartCoroutine(updateView());
    }


    public IEnumerator updateMesh(MeshInstance instance) {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        instance.mesh.subMeshCount = 11;
        instance.trianglesLenth = new int[11];
        instance.triangles = new int[11, 200000];

        stopwatch.Stop();
        UnityEngine.Debug.Log("Setting variables takes: " + stopwatch.ElapsedMilliseconds + "ms!");
        
        
        stopwatch = new Stopwatch();
        stopwatch.Start();

        int maxValue = (int) Math.Pow(WorldGenerator.chunkSize, 2) * 100;
        for(int blockId = 0; blockId < maxValue; blockId++) {
            if(instance.blockIds[blockId] != 0) {
                if((blockId - 1 > maxValue && blockId + 1 < maxValue && blockId - WorldGenerator.chunkSize > 0 && blockId + WorldGenerator.chunkSize < maxValue && blockId - (int) Math.Pow(WorldGenerator.chunkSize, 2) > 0 && blockId + (int) Math.Pow(WorldGenerator.chunkSize, 2) < maxValue)) {
                    if(
                        !((instance.blockIds[blockId - 1] != 0 && instance.blockIds[blockId + 1] != 0 &&
                        instance.blockIds[blockId - WorldGenerator.chunkSize] != 0 && instance.blockIds[blockId + WorldGenerator.chunkSize] != 0 &&
                        instance.blockIds[blockId - (int) Math.Pow(WorldGenerator.chunkSize, 2)] != 0 && instance.blockIds[blockId + (int) Math.Pow(WorldGenerator.chunkSize, 2)] != 0))
                    ) {
                        instance.updateTrianglesArray(blockId);
                    }
                } else {
                    
                }
                
                
            }

        }


        stopwatch.Stop();
        UnityEngine.Debug.Log("Calculating triangles array takes: " + stopwatch.ElapsedMilliseconds + "ms!");
        
        stopwatch = new Stopwatch();
        stopwatch.Start();

        foreach(int conn in instance.blockIdConnected) {
            instance.updateTrianglesArray(conn);
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log("Updating connections takes: " + stopwatch.ElapsedMilliseconds + "ms!");


        stopwatch = new Stopwatch();
        stopwatch.Start();

        for(int i = 0; i < 10; i++) {
            int[] resizedTriangles = new int[instance.trianglesLenth[i]];
            for(int a = 0; a < instance.trianglesLenth[i]; a++) {
                resizedTriangles[a] = instance.triangles[i, a];
            }
            instance.mesh.SetTriangles(resizedTriangles, i + 1);
        }
        //GameObject.Find("Chunk(" + instance.chunkX + "," + instance.chunkZ + ")").GetComponent<MeshCollider>().sharedMesh = instance.mesh;
        stopwatch.Stop();
        UnityEngine.Debug.Log("Setting triangles takes: " + stopwatch.ElapsedMilliseconds + "ms!");


        stopwatch = new Stopwatch();
        stopwatch.Start();

        //instance.chunkObject.GetComponent<MeshCollider>().sharedMesh = instance.mesh;
        
        stopwatch.Stop();
        UnityEngine.Debug.Log("Setting collider takes: " + stopwatch.ElapsedMilliseconds + "ms!");


        yield return null;
    }









    public IEnumerator updateView() {
        for(;;) {
            GameObject player = WorldGenerator.player;
            int renderDistance = WorldGenerator.renderDistance;

            float px = player.transform.position.x;
            float pz = player.transform.position.z;
            
            int chX = (int) (px / WorldGenerator.chunkSize);
            int chZ = (int) (pz / WorldGenerator.chunkSize);
            if(px < 0) {
            chX -= 1;   
            }

            if(pz < 0) {
            chZ -= 1;   
            }

            playerChunkX = chX;
            playerChunkZ = chZ;

            

            for(int x = playerChunkX - renderDistance; x < playerChunkX + renderDistance; x++) {
                for(int z = playerChunkZ - renderDistance; z < playerChunkZ + renderDistance; z++) {
                    Vector2 chunk = new Vector2(x, z);
                    if(!WorldGenerator.chunksLoaded.Contains(chunk) && !WorldGenerator.chunksQueued.Contains(chunk)) {
                        WorldGenerator.chunksQueued.Enqueue(new Vector2(x, z));
                    }
                }
            }
                


                if(WorldGenerator.chunksQueued.Count != 0) {
                    Vector2 chunk = WorldGenerator.chunksQueued.Dequeue();
                    WorldGenerator.GenerateChunk((int) chunk.x, (int) chunk.y);
                }


            

            int i = 0;
            foreach(MeshInstance instance in MeshInstance.meshList)
            {
                if(instance != null) {
                    if((instance.chunkX < playerChunkX - renderDistance || instance.chunkX > playerChunkX + renderDistance) || (instance.chunkZ < playerChunkZ - renderDistance || instance.chunkZ > playerChunkZ + renderDistance)) {
                        //Vector2 oldChunk = new Vector2(instance.chunkX, instance.chunkZ);
                        int a = 0;
                        foreach(Vector2 oldChunk in WorldGenerator.chunksLoaded) {
                            if(oldChunk.x == instance.chunkX && oldChunk.y == instance.chunkZ) {
                                WorldGenerator.chunksLoaded.Remove(oldChunk);
                                break;
                            }
                            a++;
                        }
                        //Destroy(GameObject.Find("Chunk(" + instance.chunkX + "," + instance.chunkZ + ")"));
                        WorldGenerator.chunkObjectsUnloaded.Add(GameObject.Find("Chunk(" + instance.chunkX + "," + instance.chunkZ + ")"));
                        MeshInstance.meshList[i] = null;
                        MeshPooling.meshPrefabs[i].triangles = null;
                        break;
                    }
                }
                i++;
            }
            UnityEngine.Debug.Log("Null instance count:" + i);



            

            yield return new WaitForSeconds(0.01f);
        }

            
    }

    
}
