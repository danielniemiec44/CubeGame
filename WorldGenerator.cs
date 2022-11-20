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

    //Vector3[] cubeVertices;
    //int[] cubeTriangles;
    //int cubeVerticesCount;
    //int cubeTrianglesCount;





    // Start is called before the first frame update
    void Start()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 0, 0);
        cube.transform.localScale = new Vector3(1, 1, 1);
        cubeMesh = cube.GetComponent<MeshFilter>().mesh;
        cubeMesh.Optimize();
        cubeMesh.RecalculateNormals();
        Vector3[] cubeVertices = cubeMesh.vertices;
        int[] cubeTriangles = cubeMesh.triangles;
        //cubeVerticesCount = cubeVertices.Length;
        //cubeTrianglesCount = cubeTriangles.Length;
        
        Debug.Log(String.Join(", ", cubeVertices));
        Debug.Log(String.Join(", ", cubeTriangles));

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

        for(int x = -8; x < 8; x++) {
            for(int z = -8; z < 8; z++) {
                if(!chunksLoaded.Contains(new Vector2(x, z))) {
                    chunksQueued.Enqueue(new Vector2(x, z));
                }
            }
        }
        
    }












    // Update is called once per frame
    void Update()
    {
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


        for(int x = 0; x < 1; x++) {
                for(int z = 0; z < 1; z++) {
                    for(int y = 0; y < 1; y++) {
                        //(int) (heightMap[noiseMapPosition] * 10)
                        
                        /*
                        vertices[cubeNumber * 24] = new Vector3 (x, y, z);
                        vertices[(cubeNumber * 24) + 1] = new Vector3 (x + 1, y, z);
                        vertices[(cubeNumber * 24) + 2] = new Vector3 (x + 1, y + 1, z);
                        vertices[(cubeNumber * 24) + 3] = new Vector3 (x, y + 1, z);
                        vertices[(cubeNumber * 24) + 4] = new Vector3 (x, y + 1, z + 1);
                        vertices[(cubeNumber * 24) + 5] = new Vector3 (x + 1, y + 1, z + 1);
                        vertices[(cubeNumber * 24) + 6] = new Vector3 (x + 1, y, z + 1);
                        vertices[(cubeNumber * 24) + 7] = new Vector3 (x, y, z + 1);
                        verticesCount += 8;
                        */


                        


                        //(0.50, -0.50, 0.50), (0.50, 0.50, 0.50), (-0.50, 0.50, 0.50), (-0.50, -0.50, 0.50), (0.50, 0.50, 0.50), (0.50, 0.50, -0.50),
                        //(-0.50, 0.50, -0.50), (-0.50, 0.50, 0.50), (0.50, 0.50, -0.50), (0.50, -0.50, -0.50), (-0.50, -0.50, -0.50), (-0.50, 0.50, -0.50),
                        //(0.50, -0.50, -0.50), (0.50, -0.50, 0.50), (-0.50, -0.50, 0.50), (-0.50, -0.50, -0.50), (-0.50, -0.50, 0.50), (-0.50, 0.50, 0.50), 
                        //(-0.50, 0.50, -0.50), (-0.50, -0.50, -0.50), (0.50, -0.50, -0.50), (0.50, 0.50, -0.50), (0.50, 0.50, 0.50), (0.50, -0.50, 0.50)
                        vertices[(cubeNumber * 24)] = new Vector3(x + 0.50f,y +  -0.50f,z + 0.50f); //0
                        vertices[(cubeNumber * 24) + 1] = new Vector3(x + 0.50f,y + 0.50f,z +  0.50f); //1
                        vertices[(cubeNumber * 24) + 2] = new Vector3(x + -0.50f,y + 0.50f,z +  0.50f); //2
                        vertices[(cubeNumber * 24) + 3] = new Vector3(x + -0.50f,y + -0.50f,z +  0.50f); //3
                        vertices[(cubeNumber * 24) + 4] = new Vector3(x + 0.50f,y + 0.50f,z +  0.50f); //4
                        vertices[(cubeNumber * 24) + 5] = new Vector3(x + 0.50f,y + 0.50f,z +  -0.50f); //5
                        vertices[(cubeNumber * 24) + 6] = new Vector3(x + -0.50f,y + 0.50f,z +  -0.50f); //6
                        vertices[(cubeNumber * 24) + 7] = new Vector3(x + -0.50f,y + 0.50f,z +  0.50f); //7
                        vertices[(cubeNumber * 24) + 8] = new Vector3(x + 0.50f,y + 0.50f,z +  -0.50f); //8
                        vertices[(cubeNumber * 24) + 9] = new Vector3(x + 0.50f,y + -0.50f,z +  -0.50f); //9
                        vertices[(cubeNumber * 24) + 10] = new Vector3(x + -0.50f,y + -0.50f,z +  -0.50f); //10
                        vertices[(cubeNumber * 24) + 11] = new Vector3(x + -0.50f,y + 0.50f,z +  -0.50f); //11
                        vertices[(cubeNumber * 24) + 12] = new Vector3(x + 0.50f,y + -0.50f,z +  -0.50f); //12
                        vertices[(cubeNumber * 24) + 13] = new Vector3(x + 0.50f,y + -0.50f,z +  0.50f); //13
                        vertices[(cubeNumber * 24) + 14] = new Vector3(x + -0.50f,y + -0.50f,z +  0.50f); //14
                        vertices[(cubeNumber * 24) + 15] = new Vector3(x + -0.50f,y + -0.50f,z +  -0.50f); //15
                        vertices[(cubeNumber * 24) + 16] = new Vector3(x + -0.50f,y + -0.50f,z +  0.50f); //16
                        vertices[(cubeNumber * 24) + 17] = new Vector3(x + -0.50f,y + 0.50f,z +  0.50f); //17
                        vertices[(cubeNumber * 24) + 18] = new Vector3(x + -0.50f,y + 0.50f,z +  -0.50f); //18
                        vertices[(cubeNumber * 24) + 19] = new Vector3(x + -0.50f,y + -0.50f,z +  -0.50f); //19
                        vertices[(cubeNumber * 24) + 20] = new Vector3(x + 0.50f,y + -0.50f,z +  -0.50f); //20
                        vertices[(cubeNumber * 24) + 21] = new Vector3(x + 0.50f,y + 0.50f,z +  -0.50f); //21
                        vertices[(cubeNumber * 24) + 22] = new Vector3(x + 0.50f,y + 0.50f,z +  0.50f); //22
                        vertices[(cubeNumber * 24) + 23] = new Vector3(x + 0.50f,y + -0.50f,z +  0.50f); //23
                        verticesCount += 24;

                        

                        //0, 1, 2, 0, 2, 3, 4, 5, 6, 4, 6, 7, 8, 9, 10, 8, 10, 11, 12, 13, 14, 12, 14, 15, 16, 17, 18, 16, 18, 19, 20, 21, 22, 20, 22, 23                        
                        triangles[cubeNumber * 36] = cubeNumber * 24;
                        triangles[(cubeNumber * 24) + 1] = 1 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 2] = 2 + (cubeNumber * 24);

                        triangles[(cubeNumber * 24) + 3] = 0 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 4] = 2 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 5] = 3 + (cubeNumber * 24);

                        triangles[(cubeNumber * 24) + 6] = 4 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 7] = 5 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 8] = 6 + (cubeNumber * 24);

                        triangles[(cubeNumber * 24) + 9] = 4 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 10] = 6 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 11] = 7 + (cubeNumber * 24);

                        triangles[(cubeNumber * 24) + 12] = 8 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 13] = 9 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 14] = 10 + (cubeNumber * 24);

                        triangles[(cubeNumber * 24) + 15] = 8 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 16] = 10 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 17] = 11 + (cubeNumber * 24);

                        triangles[(cubeNumber * 24) + 18] = 12 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 19] = 13 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 20] = 14 + (cubeNumber * 24);

                        triangles[(cubeNumber * 24) + 21] = 12 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 22] = 14 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 23] = 15 + (cubeNumber * 24);
                        
                        triangles[(cubeNumber * 24) + 24] = 16 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 25] = 17 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 26] = 18 + (cubeNumber * 24);

                        triangles[(cubeNumber * 24) + 27] = 16 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 28] = 18 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 29] = 19 + (cubeNumber * 24);

                        triangles[(cubeNumber * 24) + 30] = 20 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 31] = 21 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 32] = 22 + (cubeNumber * 24);

                        triangles[(cubeNumber * 24) + 33] = 20 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 34] = 22 + (cubeNumber * 24);
                        triangles[(cubeNumber * 24) + 35] = 23 + (cubeNumber * 24);
                    
                        trianglesCount += 24; 
                        
                        

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
            mesh.Optimize ();
            mesh.RecalculateNormals ();

            meshCollider.sharedMesh = mesh;
            //GetComponent<Renderer>().material.mainTexture = texture;

            meshList.Add(new MeshInstance(mesh, chunkX, chunkZ));

            chunksLoaded.Add(new Vector2(chunkX, chunkZ));
            //Debug.Log("Loaded chunk: " + chunkX + ", " + chunkZ + "\nChunks Queued: " + chunksQueued.Count);
    }


    float[] CalculateHeights(Vector2[] map) {
        return NoiseS3D.NoiseArrayGPU(map, 0.01f, true);
    }



}