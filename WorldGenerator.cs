using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Heading
{
    North,
    East,
    South,
    West
}

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
    public Material material2;
    
    MeshInstance[] meshList;
    int meshCount = 0;

    Mesh cubeMesh;
    RenderParams rp;
    RenderParams rp2;
    int renderDistance = 2;

    //Vector3[] cubeVertices;
    //int[] cubeTriangles;
    //int cubeVerticesCount;
    //int cubeTrianglesCount;


    [Header("Debug")]
    [SerializeField] [Range(0f, 360f)] private float northHeading;

    [Header("OutputValues")]
    [SerializeField] private float myHeading;
    [SerializeField] private float dif;
    [SerializeField] private Heading heading;





    // Start is called before the first frame update
    void Start()
    {
        rp = new RenderParams(material);
        rp2 = new RenderParams(material2);
        meshList = new MeshInstance[6000];


        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 0, 0);
        cube.transform.localScale = new Vector3(1, 1, 1);
        cubeMesh = cube.GetComponent<MeshFilter>().mesh;
        cubeMesh.Optimize();
        cubeMesh.RecalculateNormals();
        Vector3[] cubeVertices = cubeMesh.vertices;
        int[] cubeTriangles = cubeMesh.triangles;
        Vector3[] cubeNormals = cubeMesh.normals;
        Vector2[] cubeUv = cubeMesh.uv;
        //cubeVerticesCount = cubeVertices.Length;
        //cubeTrianglesCount = cubeTriangles.Length;
        
        //Debug.Log(String.Join(", ", cubeVertices));
        //Debug.Log(String.Join(", ", cubeTriangles));
        //Debug.Log(String.Join(", ", cubeNormals));
        Debug.Log(String.Join(", ", cubeUv));

        //cube.GetComponent<Renderer>().material = material;
        Destroy(cube);

        chunksLoaded = new List<Vector2>();
        chunksQueued = new Queue<Vector2>();
        canvas = GameObject.Find("Canvas");
        menu = canvas.GetComponent<Menu>();
        
        for(int x = -2; x < 2; x++) {
            for(int z = -2; z < 2; z++) {
                GenerateChunk(x, z);
            }
        }

        player.transform.position = new Vector3(0, 20, 0);
        
        menu.hideBackground();
        
    }












    // Update is called once per frame
    void Update()
    {
        int playerChunkX = (int) (player.transform.position.x / 16);
        int playerChunkZ = (int) (player.transform.position.z / 16);

        
        for(int x = playerChunkX - renderDistance; x < playerChunkX + renderDistance; x++) {
            for(int z = playerChunkZ - renderDistance; z < playerChunkZ + renderDistance; z++) {
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








        // only use the Y component of the objects orientation
        // always returns a value between 0 and 360
        myHeading = player.transform.eulerAngles.y;
        // also this is always a value between 0 and 360
        northHeading = Input.compass.magneticHeading;

        dif = myHeading - northHeading;
        // wrap the value so it is always between 0 and 360
        if (dif < 0) dif += 360f;

        if (dif > 45 && dif <= 135)
        {
            heading = Heading.East;
        }
        else if (dif > 135 && dif <= 225)
        {
            heading = Heading.South;
        }
        else if (dif > 225 && dif <= 315)
        {
            heading = Heading.West;
        }
        else
        {
            heading = Heading.North;
        }










        
        
        //foreach(MeshInstance meshInstance in meshList) {
            for(int i = 0; i < meshCount; i++){
                if(heading == Heading.East) {
                MeshInstance meshInstance = meshList[i];
                    if(meshInstance.chunkX + 1 >= playerChunkX && meshInstance.chunkX < playerChunkX + renderDistance) {
                        int cubesCount = meshInstance.cubesCount;
                        for(int meshIndex = 0; meshIndex < cubesCount; meshIndex++) {
                        //Graphics.DrawMesh(meshInstance.mesh, new Vector3(meshInstance.chunkX * 16, 0, meshInstance.chunkZ * 16), Quaternion.identity, material, meshIndex);
                            Graphics.RenderMesh(rp, meshInstance.mesh, meshIndex, Matrix4x4.Translate(new Vector3(meshInstance.chunkX * 16, 0, meshInstance.chunkZ * 16)));
                        }
                    }
                } else if(heading == Heading.West) {
                MeshInstance meshInstance = meshList[i];
                    if(meshInstance.chunkX - 1 <= playerChunkX && meshInstance.chunkX < playerChunkX + renderDistance) {
                        int cubesCount = meshInstance.cubesCount;
                        for(int meshIndex = 0; meshIndex < cubesCount; meshIndex++) {
                        //Graphics.DrawMesh(meshInstance.mesh, new Vector3(meshInstance.chunkX * 16, 0, meshInstance.chunkZ * 16), Quaternion.identity, material, meshIndex);
                            Graphics.RenderMesh(rp, meshInstance.mesh, meshIndex, Matrix4x4.Translate(new Vector3(meshInstance.chunkX * 16, 0, meshInstance.chunkZ * 16)));
                        }
                    }
                } else if(heading == Heading.North) {
                MeshInstance meshInstance = meshList[i];
                    if(meshInstance.chunkZ + 1 >= playerChunkZ && meshInstance.chunkZ < playerChunkZ + renderDistance) {
                        int cubesCount = meshInstance.cubesCount;
                        for(int meshIndex = 0; meshIndex < cubesCount; meshIndex++) {
                        //Graphics.DrawMesh(meshInstance.mesh, new Vector3(meshInstance.chunkX * 16, 0, meshInstance.chunkZ * 16), Quaternion.identity, material, meshIndex);
                            Graphics.RenderMesh(rp, meshInstance.mesh, meshIndex, Matrix4x4.Translate(new Vector3(meshInstance.chunkX * 16, 0, meshInstance.chunkZ * 16)));
                        }
                    }
                } else if(heading == Heading.South) {
                MeshInstance meshInstance = meshList[i];
                    if(meshInstance.chunkZ - 1 <= playerChunkZ && meshInstance.chunkZ < playerChunkZ + renderDistance) {
                        int cubesCount = meshInstance.cubesCount;
                        for(int meshIndex = 0; meshIndex < cubesCount; meshIndex++) {
                        //Graphics.DrawMesh(meshInstance.mesh, new Vector3(meshInstance.chunkX * 16, 0, meshInstance.chunkZ * 16), Quaternion.identity, material, meshIndex);
                            Graphics.RenderMesh(rp, meshInstance.mesh, meshIndex, Matrix4x4.Translate(new Vector3(meshInstance.chunkX * 16, 0, meshInstance.chunkZ * 16)));
                        }
                    }
                }
            }
            
        //}
            }






    void GenerateChunk(int chunkX, int chunkZ)
    {

        Vector3[] vertices = new Vector3[600000];
        int[] triangles = new int[600000];
        Vector3[] normals = new Vector3[600000];
        Vector2[] uv = new Vector2[600000];
        int verticesCount = 0;
        int trianglesCount = 0;
        int uvCount = 0;

        //int cubeNumber = 0;
        GameObject chunk = Instantiate(chunkPrefab, new Vector3(chunkX * 16, 0, chunkZ * 16), Quaternion.identity);
        //MeshFilter meshFilter = chunk.GetComponent<MeshFilter> ();
        //Renderer renderer = chunk.GetComponent<Renderer>();
        MeshCollider meshCollider = chunk.GetComponent<MeshCollider>();

        Mesh mesh = new Mesh();

        Vector2[] noiseMap = new Vector2[600000];
        int i = 0;
        for(int x = 0; x < 16; x++) {
            for(int z = 0; z < 16; z++) {
                noiseMap[i] = new Vector2(x + (chunkX * 16), (chunkZ * 16) + z);
                i++;
            }
        }

        float[] heightMap = CalculateHeights(noiseMap);


        int noiseMapPosition = 0;
        int cubeNumber = 0;
        for(int x = 0; x < 16; x++) {
                for(int z = 0; z < 16; z++) {
                    //for(int y = 0; y < (int) (heightMap[noiseMapPosition] * 10); y++) {
                    int y = (int) (heightMap[noiseMapPosition] * 10);
                        //(int) (heightMap[noiseMapPosition] * 10)
            vertices[cubeNumber * 14] = new Vector3(x + 0, y + 1, z + 0);
            vertices[(cubeNumber * 14) + 1] = new Vector3(x + 0, y + 0, z + 0);
            vertices[(cubeNumber * 14) + 2] = new Vector3(x + 1, y + 1, z + 0);
            vertices[(cubeNumber * 14) + 3] = new Vector3(x + 1, y + 0, z + 0);
            vertices[(cubeNumber * 14) + 4] = new Vector3(x + 0, y + 0, z + 1);
            vertices[(cubeNumber * 14) + 5] = new Vector3(x + 1, y + 0, z + 1);
            vertices[(cubeNumber * 14) + 6] = new Vector3(x + 0, y + 1, z + 1);
            vertices[(cubeNumber * 14) + 7] = new Vector3(x + 1, y + 1, z + 1);

            vertices[(cubeNumber * 14) + 8] = new Vector3(x + 0, y + 1, z + 0);
            vertices[(cubeNumber * 14) + 9] = new Vector3(x + 1, y + 1, z + 0);
            vertices[(cubeNumber * 14) + 10] = new Vector3(x + 0, y + 1, z + 0);
            vertices[(cubeNumber * 14) + 11] = new Vector3(x + 0, y + 1, z + 1);
            vertices[(cubeNumber * 14) + 12] = new Vector3(x + 1, y + 1, z + 0);
            vertices[(cubeNumber * 14) + 13] = new Vector3(x + 1, y + 1, z + 1);
            verticesCount += 14;




            triangles[(cubeNumber * 36) + 0] = 0 + cubeNumber * 14;
            triangles[(cubeNumber * 36) + 1] = 1 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 2] = 2 + (cubeNumber * 14);

            triangles[(cubeNumber * 36) + 3] = 2 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 4] = 1 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 5] = 3 + (cubeNumber * 14);

            triangles[(cubeNumber * 36) + 6] = 1 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 7] = 4 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 8] = 3 + (cubeNumber * 14);

            triangles[(cubeNumber * 36) + 9] = 3 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 10] = 4 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 11] = 5 + (cubeNumber * 14);

            triangles[(cubeNumber * 36) + 12] = 4 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 13] = 6 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 14] = 5 + (cubeNumber * 14);

            triangles[(cubeNumber * 36) + 15] = 5 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 16] = 6 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 17] = 7 + (cubeNumber * 14);

            triangles[(cubeNumber * 36) + 18] = 6 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 19] = 7 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 20] = 8 + (cubeNumber * 14);

            triangles[(cubeNumber * 36) + 21] = 8 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 22] = 7 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 23] = 9 + (cubeNumber * 14);
            
            triangles[(cubeNumber * 36) + 24] = 1 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 25] = 11 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 26] = 10 + (cubeNumber * 14);

            triangles[(cubeNumber * 36) + 27] = 1 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 28] = 4 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 29] = 11 + (cubeNumber * 14);

            triangles[(cubeNumber * 36) + 30] = 3 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 31] = 12 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 32] = 5 + (cubeNumber * 14);

            triangles[(cubeNumber * 36) + 33] = 5 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 34] = 12 + (cubeNumber * 14);
            triangles[(cubeNumber * 36) + 35] = 13 + (cubeNumber * 14);
        
            trianglesCount += 36;



            






            uv[(cubeNumber * 14)] = new Vector2(x + 0, z + 0.66f); //0
            uv[(cubeNumber * 14) + 1] = new Vector2(x + 0.25f, z + 0.66f); //1
            uv[(cubeNumber * 14) + 2] = new Vector2(x + 0, z + 0.33f); //2
            uv[(cubeNumber * 14) + 3] = new Vector2(x + 0.25f, z + 0.33f); //3
            uv[(cubeNumber * 14) + 4] = new Vector2(x + 0.5f, z + 0.66f); //4
            uv[(cubeNumber * 14) + 5] = new Vector2(x + 0.5f, z + 0.33f); //5
            uv[(cubeNumber * 14) + 6] = new Vector2(x + 0.75f, z + 0.66f); //6
            uv[(cubeNumber * 14) + 7] = new Vector2(x + 0.75f, z + 0.33f); //7
            uv[(cubeNumber * 14) + 8] = new Vector2(x + 1, z + 0.66f); //8
            uv[(cubeNumber * 14) + 9] = new Vector2(x + 1, z + 0.33f); //9
            uv[(cubeNumber * 14) + 10] = new Vector2(x + 0.25f, z + 1); //10
            uv[(cubeNumber * 14) + 11] = new Vector2(x + 0.5f, z + 1); //11
            uv[(cubeNumber * 14) + 12] = new Vector2(x + 0.25f, z + 0); //12
            uv[(cubeNumber * 14) + 13] = new Vector2(x + 0.5f, z + 0); //13

            uvCount += 14;

            cubeNumber++;
            cubesCountTotal++;
            mesh.subMeshCount++;
            

                //}
                noiseMapPosition++;
            }
        }
        Array.Resize(ref vertices, verticesCount);
        Array.Resize(ref triangles, trianglesCount);
        Array.Resize(ref uv, uvCount);
        mesh.vertices = vertices;
        for(int triangleNumber = 0; triangleNumber < (triangles.Length / 72); triangleNumber++) {
            mesh.SetTriangles(triangles, triangleNumber * 36, (triangleNumber + 1) * 36, triangleNumber);
        }


        //mesh.triangles = triangles;



        //mesh.SetTriangles(triangles, 0, 36, 0);
        //mesh.SetTriangles(triangles, 36, 72, 1);
        //mesh.SetTriangles(triangles, 72, 108, 2);
        //mesh.SetTriangles(triangles, 108, 144, 3);
        
        //mesh.SetTriangles(triangles, 36, 72, 1);
        mesh.normals = vertices;
        mesh.uv = uv;
        mesh.Optimize ();
		mesh.RecalculateNormals ();

        meshCollider.sharedMesh = mesh;
        //GetComponent<Renderer>().material.mainTexture = texture;

        meshList[meshCount] = new MeshInstance(mesh, chunkX, chunkZ, cubeNumber);
        meshCount++;

        chunksLoaded.Add(new Vector2(chunkX, chunkZ));
        //Debug.Log("Loaded chunk: " + chunkX + ", " + chunkZ + "\nChunks Queued: " + chunksQueued.Count);
    }


    float[] CalculateHeights(Vector2[] map) {
        return NoiseS3D.NoiseArrayGPU(map, 0.01f, true);
    }



}