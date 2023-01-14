using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Rendering;

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

    int framesInterval = 0;
    int framesBehind = 0;

    public Material material;
    public Material material2;
    
    static MeshInstance[] meshList;

    Mesh cubeMesh;
    RenderParams[] rps;
    int renderDistance = 4;

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

    public MaterialConstructor materialConstructor;





    int standardHeight = 7;
    float[,,] heightMap = new float[2000,2000,256];

    static int playerChunkX;
    static int playerChunkZ;

    

    





    // Start is called before the first frame update
    void Start()
    {
        meshList = new MeshInstance[600];

        chunksLoaded = new List<Vector2>();
        chunksQueued = new Queue<Vector2>();
        canvas = GameObject.Find("Canvas");
        menu = canvas.GetComponent<Menu>();
        
        

        player.transform.position = new Vector3(0, 20, 0);
        
        menu.hideBackground();
        
        materialConstructor = GameObject.Find("Materials").GetComponent<MaterialConstructor>();
        rps = new RenderParams[11];
        rps[0] = new RenderParams(materialConstructor.materials[0]);
        rps[1] = new RenderParams(materialConstructor.materials[1]);
        rps[2] = new RenderParams(materialConstructor.materials[2]);
        rps[3] = new RenderParams(materialConstructor.materials[3]);
        rps[4] = new RenderParams(materialConstructor.materials[4]);
        rps[5] = new RenderParams(materialConstructor.materials[5]);
        rps[6] = new RenderParams(materialConstructor.materials[6]);
        rps[7] = new RenderParams(materialConstructor.materials[7]);
        rps[8] = new RenderParams(materialConstructor.materials[8]);
        rps[9] = new RenderParams(materialConstructor.materials[9]);
        rps[10] = new RenderParams(materialConstructor.materials[10]);

        GenerateHeightMap();

        for(int x = -renderDistance; x < renderDistance; x++) {
            for(int z = -renderDistance; z < renderDistance; z++) {
                GenerateChunk(x, z);
            }
        }

    }



    // Update is called once per frame
    void Update()
    {
        
        playerChunkX = (int) (player.transform.position.x / 16);
        playerChunkZ = (int) (player.transform.position.z / 16);

        


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


        

        int i = 0;
        foreach(MeshInstance instance in meshList)
        {
            if(instance != null) {
                if((instance.chunkX < playerChunkX - renderDistance || instance.chunkX > playerChunkX + renderDistance) || (instance.chunkZ < playerChunkZ - renderDistance || instance.chunkZ > playerChunkZ + renderDistance)) {
                    //Vector2 oldChunk = new Vector2(instance.chunkX, instance.chunkZ);
                    int a = 0;
                    foreach(Vector2 oldChunk in chunksLoaded) {
                        if(oldChunk.x == instance.chunkX && oldChunk.y == instance.chunkZ) {
                            chunksLoaded.Remove(oldChunk);
                            break;
                        }
                        a++;
                    }
                    Destroy(GameObject.Find("Chunk(" + instance.chunkX + "," + instance.chunkZ + ")"));
                    meshList[i] = null;
                    MeshPooling.meshPrefabs[i].triangles = null;
                    break;
                }
            }
            i++;
        }






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

            foreach(MeshInstance meshInstance in meshList){
                if(meshInstance != null) {
                    for(int submeshIndex = 1; submeshIndex < meshInstance.mesh.subMeshCount; submeshIndex++) {
                        Graphics.RenderMesh(rps[submeshIndex - 1], meshInstance.mesh, submeshIndex, Matrix4x4.Translate(new Vector3(meshInstance.chunkX * 16, 0, meshInstance.chunkZ * 16)));
                    }
                }
            }
            
        //}
            }






    void GenerateChunk(int chunkX, int chunkZ)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        //int cubeNumber = 0;
        GameObject chunk = Instantiate(chunkPrefab, new Vector3(chunkX * 16, 0, chunkZ * 16), Quaternion.identity);
        chunk.name = "Chunk(" + chunkX + "," + chunkZ + ")";
        //MeshFilter meshFilter = chunk.GetComponent<MeshFilter> ();
        //Renderer renderer = chunk.GetComponent<Renderer>();
        MeshCollider meshCollider = chunk.GetComponent<MeshCollider>();
        watch.Stop();
        Debug.Log("Instantiating takes: " + watch.ElapsedMilliseconds + "ms");
        
        watch = System.Diagnostics.Stopwatch.StartNew();
        
        
        watch.Stop();
        Debug.Log("Calculating heightmap takes: " + watch.ElapsedMilliseconds + "ms");

        
        watch = System.Diagnostics.Stopwatch.StartNew();
        
        int[] meshTriangles = new int[600000];
        int[] meshDirtTriangles = new int[600000];
        int[] meshBedrockTriangles = new int[600000];
        int[] meshStoneTriangles = new int[600000];
        int trianglesCount = 0;
        int dirtTrianglesCount = 0;
        int bedrockTrianglesCount = 0;
        int stoneTrianglesCount = 0;
        int heightMapPosition = 0;
        
        for(int x = 0; x < 16; x++) {
            for(int z = 0; z < 16; z++) {
                Array.Copy(calculateTriangles(calculateCubeIndex(new Vector3(x, (int) (heightMap[chunkX + 1000, chunkZ + 1000, heightMapPosition] * 10) + standardHeight, z))), 0, meshTriangles, trianglesCount, 36);
                trianglesCount += 36;
                Array.Copy(calculateTriangles(calculateCubeIndex(new Vector3(x, 0, z))), 0, meshBedrockTriangles, bedrockTrianglesCount, 36);
                bedrockTrianglesCount += 36;
                for(int y = 1; y < standardHeight; y++) {
                    Array.Copy(calculateTriangles(calculateCubeIndex(new Vector3(x, y, z))), 0, meshStoneTriangles, stoneTrianglesCount, 36);
                    stoneTrianglesCount += 36;
                }
                for(int y = standardHeight; y < (int) (heightMap[chunkX + 1000, chunkZ + 1000, heightMapPosition] * 10) + standardHeight; y++) {
                    Array.Copy(calculateTriangles(calculateCubeIndex(new Vector3(x, y, z))), 0, meshDirtTriangles, dirtTrianglesCount, 36);
                    dirtTrianglesCount += 36;
                }
                heightMapPosition++;
            }
        }
        
        
        //Array.Copy(calculateTriangles(0), 0, meshTriangles, 0, 36);
        //Array.Copy(calculateTriangles(calculateCubeIndex(new Vector3(0, 0, 10))), 0, meshTriangles, 36, 36);
        Array.Resize(ref meshTriangles, trianglesCount);
        Array.Resize(ref meshDirtTriangles, dirtTrianglesCount);
        Array.Resize(ref meshBedrockTriangles, bedrockTrianglesCount);
        Array.Resize(ref meshStoneTriangles, stoneTrianglesCount);
        watch.Stop();
        Debug.Log("Calculating triangles takes: " + watch.ElapsedMilliseconds + "ms");



        watch = System.Diagnostics.Stopwatch.StartNew();
        
        watch.Stop();
        int meshCount = getFreeMesh();
        Mesh mesh = MeshPooling.meshPrefabs[meshCount];
        Debug.Log("Setting mesh takes: " + watch.ElapsedMilliseconds + "ms");



        watch = System.Diagnostics.Stopwatch.StartNew();
        mesh.subMeshCount = 11;
        mesh.SetTriangles(meshTriangles, 1);
        mesh.SetTriangles(meshDirtTriangles, 2);
        mesh.SetTriangles(meshBedrockTriangles, 3);
        mesh.SetTriangles(meshStoneTriangles, 4);
        
        mesh.RecalculateNormals ();
        watch.Stop();
        Debug.Log("Setting triangles takes: " + watch.ElapsedMilliseconds + "ms");


        
        watch = System.Diagnostics.Stopwatch.StartNew();
        meshCollider.sharedMesh = mesh;
        watch.Stop();
        Debug.Log("Setting collider takes: " + watch.ElapsedMilliseconds + "ms");
    


        meshList[meshCount] = new MeshInstance(mesh, chunkX, chunkZ);

        chunksLoaded.Add(new Vector2(chunkX, chunkZ));


        
        
    }





    float[] CalculateHeights(Vector2[] map) {
        return NoiseS3D.NoiseArrayGPU(map, 0.01f, true);
    }


    public static void removeBlock(Vector3 block) {
        //int cubeIndex = calculateCubeIndex(block);
        //int[] newTriangles = Array.FindAll(array, i => i != item).ToArray();
        Vector2 chunkVector = getChunk(block);
        Vector3 blockVector = getBlock(block);
        int cubeIndex = calculateCubeIndex(blockVector);
        foreach(MeshInstance instance in meshList) {
            if(instance.chunkX == chunkVector.x && instance.chunkZ == chunkVector.y) {
                
                for(int subMeshIndex = 1; subMeshIndex < instance.mesh.subMeshCount; subMeshIndex++) {
                    int[] newTriangles = instance.mesh.GetTriangles(subMeshIndex);
                    for(int i = 0; i < 14; i++) {
                        newTriangles = newTriangles.Where(e => (e != (cubeIndex * 14) + i)).ToArray();
                    }
                    instance.mesh.SetTriangles(newTriangles, subMeshIndex);
                    instance.mesh.RecalculateNormals();
                }
                //Debug.Log("SubMeshCount: " + instance.mesh.subMeshCount);
                //instance.mesh.SetTriangles(calculateTriangles(calculateCubeIndex(blockVector - new Vector3(0, -1, 0))), 1);
                GameObject.Find("Chunk(" + instance.chunkX + "," + instance.chunkZ + ")").GetComponent<MeshCollider>().sharedMesh = instance.mesh;
                break;
            }
        }

    }


    public static void setBlock(Vector3 block) {
        Vector2 chunkVector = getChunk(block);
        Vector3 blockVector = getBlock(block);
        int cubeIndex = calculateCubeIndex(blockVector);
        foreach(MeshInstance instance in meshList) {
            if(instance.chunkX == chunkVector.x && instance.chunkZ == chunkVector.y) {
                int HotBarFocus = Menu.HotBarFocus + 1;
                int[] oldTriangles = instance.mesh.GetTriangles(HotBarFocus);
                int[] newTriangles = new int[oldTriangles.Length + 36];
                Array.Copy(oldTriangles, newTriangles, oldTriangles.Length);
                Array.Copy(calculateTriangles(cubeIndex), 0, newTriangles, oldTriangles.Length, 36);
                instance.mesh.SetTriangles(newTriangles, HotBarFocus);
                instance.mesh.RecalculateNormals();

                GameObject.Find("Chunk(" + instance.chunkX + "," + instance.chunkZ + ")").GetComponent<MeshCollider>().sharedMesh = instance.mesh;

                break;
            }
        }
    }

    public static int[] calculateTriangles(int cubeNumber) {
        int[] triangles = new int[36];

        triangles[0] = 0 + (cubeNumber * 14);
        triangles[1] = 2 + (cubeNumber * 14);
        triangles[2] = 1 + (cubeNumber * 14);

        triangles[3] = 1 + (cubeNumber * 14);
        triangles[4] = 2 + (cubeNumber * 14);
        triangles[5] = 3 + (cubeNumber * 14);

        triangles[6] = 1 + (cubeNumber * 14);
        triangles[7] = 4 + (cubeNumber * 14);
        triangles[8] = 3 + (cubeNumber * 14);

        triangles[9] = 3 + (cubeNumber * 14);
        triangles[10] = 4 + (cubeNumber * 14);
        triangles[11] = 5 + (cubeNumber * 14);


        triangles[12] = 4 + (cubeNumber * 14);
        triangles[13] = 5 + (cubeNumber * 14);
        triangles[14] = 6 + (cubeNumber * 14);

        triangles[15] = 5 + (cubeNumber * 14);
        triangles[16] = 7 + (cubeNumber * 14);
        triangles[17] = 6 + (cubeNumber * 14);

        triangles[18] = 6 + (cubeNumber * 14);
        triangles[19] = 7 + (cubeNumber * 14);
        triangles[20] = 8 + (cubeNumber * 14);

        triangles[21] = 7 + (cubeNumber * 14);
        triangles[22] = 9 + (cubeNumber * 14);
        triangles[23] = 8 + (cubeNumber * 14);
        
        triangles[24] = 1 + (cubeNumber * 14);
        triangles[25] = 11 + (cubeNumber * 14);
        triangles[26] = 10 + (cubeNumber * 14);

        triangles[27] = 1 + (cubeNumber * 14);
        triangles[28] = 4 + (cubeNumber * 14);
        triangles[29] = 11 + (cubeNumber * 14);

        triangles[30] = 3 + (cubeNumber * 14);
        triangles[31] = 12 + (cubeNumber * 14);
        triangles[32] = 5 + (cubeNumber * 14);

        triangles[33] = 5 + (cubeNumber * 14);
        triangles[34] = 12 + (cubeNumber * 14);
        triangles[35] = 13 + (cubeNumber * 14);



        return triangles;
    }


    public static int calculateCubeIndex(Vector3 vector) {
        int x = (int) vector.x;
        int y = (int) vector.y;
        int z = (int) vector.z;

        return x + (z * 16) + (y * 256);
    }

    public static Vector2 getChunk(Vector3 vector) {
        int chunkX = (int) ((vector.x) / 16);
        int chunkZ = (int) ((vector.z) / 16);

        if(vector.x < 0) {
            chunkX -= 1;
        }

        if(vector.z < 0) {
            chunkZ -= 1;
        }

        return new Vector2(chunkX, chunkZ);
    }

    public static Vector3 getBlock(Vector3 vector) {
        int x = (int) ((vector.x) % 16);
        int z = (int) ((vector.z) % 16);

        if(((vector.x) % 16) < 0) {
            x += 15;
        }

        if(((vector.z) % 16) < 0) {
            z += 15;
        }

        return new Vector3(x, vector.y, z);
    }

    public void GenerateHeightMap() {
        for(int chunkX = -50; chunkX < 50; chunkX++) {
            for(int chunkZ = -50; chunkZ < 50; chunkZ++) {
                int i = 0;
                Vector2[] noiseMap = new Vector2[256];
                for(int x = 0; x < 16; x++) {
                    for(int z = 0; z < 16; z++) {
                        noiseMap[i] = new Vector2(x + (chunkX * 16), (chunkZ * 16) + z);
                        i++;
                    }
                }
                float[] singleHeightMap = CalculateHeights(noiseMap);
                for(int a = 0; a < 256; a++) {
                    heightMap[chunkX + 1000, chunkZ + 1000, a] = singleHeightMap[a];
                }
            }
        }

    }

    public int getFreeMesh() {
        int i = 0;
        foreach(MeshInstance instance in meshList) {
            if(instance == null) {
                return i;
            }
            i++;
        }
        return i;
    }



}