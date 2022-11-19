using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldGenerator : MonoBehaviour
{
    public Texture texture;
    public GameObject chunkPrefab;


    //List<Vector3> verticesList;
    //List<int> trianglesList;
    //int[,,] cubes = new int[16,16,16];
    int cubesCountTotal = 0;


    public GameObject player;
    List<Vector2> chunksLoaded;
    Queue<Vector2> chunksQueued;

    GameObject canvas;

    Menu menu;

    int framesInterval = 1;
    int framesBehind = 0;

    public Material material;
    
    List<MeshInstance> meshList;

    Mesh cubeMesh;

    Vector3[] cubeVertices;
    int[] cubeTriangles;
    int cubeVerticesCount;
    int cubeTrianglesCount;





    // Start is called before the first frame update
    void Start()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 0, 0);
        cube.transform.localScale = new Vector3(1, 1, 1);
        cubeMesh = cube.GetComponent<MeshFilter>().mesh;
        cubeMesh.Optimize();
        cubeMesh.RecalculateNormals();
        cubeVertices = cubeMesh.vertices;
        cubeTriangles = cubeMesh.triangles;
        cubeVerticesCount = cubeVertices.Length;
        cubeTrianglesCount = cubeTriangles.Length;
        Debug.Log(cubeVerticesCount + " vertices, " + cubeTrianglesCount + " triangles loaded!");
        Destroy(cube);


        meshList = new List<MeshInstance>();

        chunksLoaded = new List<Vector2>();
        chunksQueued = new Queue<Vector2>();
        canvas = GameObject.Find("Canvas");
        menu = canvas.GetComponent<Menu>();
        
        for(int x = -4; x < 4; x++) {
            for(int z = -4; z < 4; z++) {
                GenerateChunk(x, z);
            }
        }

        player.transform.position = new Vector3(0, 20, 0);
        
        menu.hideBackground();

        for(int x = -1; x < 1; x++) {
            for(int z = -1; z < 1; z++) {
                if(!chunksLoaded.Contains(new Vector2(x, z))) {
                    chunksQueued.Enqueue(new Vector2(x, z));
                }
            }
        }
        
    }












    // Update is called once per frame
    void Update()
    {
        /*
        int playerChunkX = (int) (player.transform.position.x / 16);
        int playerChunkZ = (int) (player.transform.position.z / 16);

        for(int x = playerChunkX -16; x < playerChunkX + 16; x++) {
            for(int z = playerChunkZ -16; z < playerChunkZ + 16; z++) {
                Vector2 chunk = new Vector2(x, z);
                if(!chunksLoaded.Contains(chunk) && !chunksQueued.Contains(chunk)) {
                    chunksQueued.Enqueue(new Vector2(x, z));
                }
            }

            
            }
            */
            


        if(framesBehind > framesInterval) {
            if(chunksQueued.Count != 0) {
                Vector2 chunk = chunksQueued.Dequeue();
                GenerateChunk((int) chunk.x, (int) chunk.y);
            }
            framesBehind = 0;
        }
        framesBehind++;



        RenderParams rp = new RenderParams(material);
        foreach(MeshInstance meshInstance in meshList) {
            Graphics.RenderMesh(rp, meshInstance.mesh, 0, Matrix4x4.Translate(new Vector3(meshInstance.chunkX * 16, 0, meshInstance.chunkZ * 16)));
        }
    }






    void GenerateChunk(int chunkX, int chunkZ)
    {

        Vector3[] vertices = new Vector3[600000];
        int[] triangles = new int[600000];
        int verticesCount = 0;
        int trianglesCount = 0;

        int cubeNumber = 0;
        GameObject chunk = Instantiate(chunkPrefab, new Vector3(chunkX * 16, 0, chunkZ * 16), Quaternion.identity);
        //MeshFilter meshFilter = chunk.GetComponent<MeshFilter> ();
        //Renderer renderer = chunk.GetComponent<Renderer>();
        MeshCollider meshCollider = chunk.GetComponent<MeshCollider>();

        Vector2[] noiseMap = new Vector2[600000];
        int i = 0;
        for(int x = 0; x < 16; x++) {
            for(int z = 0; z < 16; z++) {
                noiseMap[i] = new Vector2((chunkX * 16) + x, (chunkZ * 16) + z);
                i++;
            }
        }

        float[] heightMap = CalculateHeights(noiseMap);


        int noiseMapPosition = 0;


        for(int x = 0; x < 16; x++) {
                for(int z = 0; z < 16; z++) {
                    for(int y = 0; y < (int) (heightMap[noiseMapPosition] * 10); y++) {
                        
                        /*
                        int verticeNumber = 0;
                        foreach(Vector3 cubeVertice in cubeVertices) {
                            Vector3 verticePosition = cubeVertices[verticeNumber];
                            verticePosition.x += x;
                            verticePosition.y += y;
                            verticePosition.z += z;
                            vertices[(cubeNumber * cubeVerticesCount) + verticeNumber] = verticePosition;
                            //Debug.Log(verticeNumber);
                            verticeNumber++;
                        }
                        */
                        

/*
                        for(int verticeNumber = 0; verticeNumber < cubeVerticesCount; verticeNumber++) {
                            Vector3 verticePosition = cubeVertices[verticeNumber];
                            verticePosition.x += 0;
                            verticePosition.y += 0;
                            verticePosition.z += 0;
                            
                            vertices[(cubeNumber * cubeVerticesCount) + verticeNumber] = verticePosition;
                            verticesCount++;
                        }
                        */


                        
                        vertices[cubeNumber * 8] = new Vector3 (x, y, z);
                        vertices[(cubeNumber * 8) + 1] = new Vector3 (x + 1, y, z);
                        vertices[(cubeNumber * 8) + 2] = new Vector3 (x + 1, y + 1, z);
                        vertices[(cubeNumber * 8) + 3] = new Vector3 (x, y + 1, z);
                        vertices[(cubeNumber * 8) + 4] = new Vector3 (x, y + 1, z + 1);
                        vertices[(cubeNumber * 8) + 5] = new Vector3 (x + 1, y + 1, z + 1);
                        vertices[(cubeNumber * 8) + 6] = new Vector3 (x + 1, y, z + 1);
                        vertices[(cubeNumber * 8) + 7] = new Vector3 (x, y, z + 1);
                        verticesCount += cubeVerticesCount;
                        

/*
                        int triangleNumber = 0;
                        foreach(int triangle in cubeTriangles) {
                            
                            triangleNumber++;
                        }
                        */

/*
                        for(int triangleNumber = 0; triangleNumber < cubeTrianglesCount; triangleNumber++) {
                            triangles[(cubeNumber * cubeTrianglesCount) + triangleNumber] = cubeTriangles[triangleNumber];
                            trianglesCount++;
                        }
                        */
                        

                        
                        triangles[cubeNumber * 36] = 0 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 1] = 2 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 2] = 1 + (cubeNumber * 8);

                        triangles[(cubeNumber * 36) + 3] = 0 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 4] = 3 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 5] = 2 + (cubeNumber * 8);

                        triangles[(cubeNumber * 36) + 6] = 2 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 7] = 3 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 8] = 4 + (cubeNumber * 8);

                        triangles[(cubeNumber * 36) + 9] = 2 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 10] = 4 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 11] = 5 + (cubeNumber * 8);

                        triangles[(cubeNumber * 36) + 12] = 1 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 13] = 2 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 14] = 5 + (cubeNumber * 8);

                        triangles[(cubeNumber * 36) + 15] = 1 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 16] = 5 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 17] = 6 + (cubeNumber * 8);

                        triangles[(cubeNumber * 36) + 18] = 0 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 19] = 7 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 20] = 4 + (cubeNumber * 8);

                        triangles[(cubeNumber * 36) + 21] = 0 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 22] = 4 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 23] = 3 + (cubeNumber * 8);
                        
                        triangles[(cubeNumber * 36) + 24] = 5 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 25] = 4 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 26] = 7 + (cubeNumber * 8);

                        triangles[(cubeNumber * 36) + 27] = 5 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 28] = 7 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 29] = 6 + (cubeNumber * 8);

                        triangles[(cubeNumber * 36) + 30] = 0 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 31] = 6 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 32] = 7 + (cubeNumber * 8);

                        triangles[(cubeNumber * 36) + 33] = 0 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 34] = 1 + (cubeNumber * 8);
                        triangles[(cubeNumber * 36) + 35] = 6 + (cubeNumber * 8);
                    
                        trianglesCount += 36; 
                        
                        

                        cubeNumber++;
                        cubesCountTotal++;

                        
                    }
                    noiseMapPosition++;
                }
            }
                
            //Mesh mesh = meshFilter.mesh;
            //mesh.Clear ();


            Array.Resize(ref vertices, verticesCount);
            Array.Resize(ref triangles, trianglesCount);
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            Debug.Log(vertices.Length + " vertices, " + triangles.Length + " triangles");
            mesh.Optimize ();
            mesh.RecalculateNormals ();

            meshCollider.sharedMesh = cubeMesh;
            //GetComponent<Renderer>().material.mainTexture = texture;

            meshList.Add(new MeshInstance(mesh, chunkX, chunkZ));

            chunksLoaded.Add(new Vector2(chunkX, chunkZ));
            //Debug.Log("Loaded chunk: " + chunkX + ", " + chunkZ + "\nChunks Queued: " + chunksQueued.Count);
    }


    float[] CalculateHeights(Vector2[] map) {
        return NoiseS3D.NoiseArrayGPU(map, 0.01f, true);
    }



}