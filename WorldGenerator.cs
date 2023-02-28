using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

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
    public GameObject prefab;
    public static GameObject chunkPrefab;

    int cubesCountTotal = 0;


    public static GameObject player;
    public static List<Vector2> chunksLoaded;
    public static Queue<Vector2> chunksQueued;

    GameObject canvas;

    Menu menu;

    int framesInterval = 10;
    int framesBehind = 0;

    public Material material;
    public Material material2;

    public static int chunkSize = 16;
    


    Mesh cubeMesh;
    public static RenderParams[] rps;
    public static int renderDistance = 2;

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





    public static int standardHeight = 70;



    public static List<GameObject> chunkObjectsUnloaded = new List<GameObject>();

    public static float[,,] heightMap = new float[100,100,(int) Math.Pow(WorldGenerator.chunkSize, 2)];

    

    





    // Start is called before the first frame update
    void Start()
    {
        
        chunkPrefab = prefab;
        chunksLoaded = new List<Vector2>();
        chunksQueued = new Queue<Vector2>();
        canvas = GameObject.Find("Canvas");
        menu = canvas.GetComponent<Menu>();
        player = GameObject.Find("Player");
        
        

        player.transform.position = new Vector3(0, 100, 0);
        
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

        

        for(int x = -5; x < 5; x++) {
            for(int z = -5; z < 5; z++) {
                GenerateChunk(x, z);
            }
        }

        MeshInstance.meshUpdater.updateViewAsync();

    }



    // Update is called once per frame
    void Update()
    {
        foreach(MeshInstance meshInstance in MeshInstance.meshList){
                if(meshInstance != null) {
                    for(int submeshIndex = 1; submeshIndex < 11; submeshIndex++) {
                        Graphics.RenderMesh(WorldGenerator.rps[submeshIndex - 1], meshInstance.mesh, submeshIndex, Matrix4x4.Translate(new Vector3(meshInstance.chunkX * chunkSize, 0, meshInstance.chunkZ * chunkSize)));
                    }
                }
            }
    }

    public void GenerateHeightMap() {
        for(int chunkX = -50; chunkX < 50; chunkX++) {
            for(int chunkZ = -50; chunkZ < 50; chunkZ++) {
                int i = 0;
                Vector2[] noiseMap = new Vector2[(int) Math.Pow(WorldGenerator.chunkSize, 2)];
                for(int x = 0; x < WorldGenerator.chunkSize; x++) {
                    for(int z = 0; z < WorldGenerator.chunkSize; z++) {
                        noiseMap[i] = new Vector2(x + (chunkX * WorldGenerator.chunkSize), (chunkZ * WorldGenerator.chunkSize) + z);
                        i++;
                    }
                }
                float[] singleHeightMap = CalculateHeights(noiseMap);
                for(int a = 0; a < (int) Math.Pow(WorldGenerator.chunkSize, 2); a++) {
                    heightMap[chunkX + 50, chunkZ + 50, a] = Mathf.Pow(singleHeightMap[a], 2.0f);
                }
            }
        }

    }

    float[] CalculateHeights(Vector2[] map) {
        return NoiseS3D.NoiseArrayGPU(map, 0.01f, true);
    }






    public static void GenerateChunk(int chunkX, int chunkZ)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        
        GameObject chunk = null;
        if(chunkObjectsUnloaded.Count == 0) {
            chunk = Instantiate(chunkPrefab, new Vector3(chunkX * chunkSize, 0, chunkZ * chunkSize), Quaternion.identity);
        } else {
            chunk = chunkObjectsUnloaded[0];
            chunk.transform.position = new Vector3(chunkX * chunkSize, 0, chunkZ * chunkSize);
            chunkObjectsUnloaded.RemoveAt(0);
        }
        chunk.name = "Chunk(" + chunkX + "," + chunkZ + ")";
        MeshCollider meshCollider = chunk.GetComponent<MeshCollider>();

        int heightMapPosition = 0;
        int[] blockIds = new int[(int) Math.Pow(chunkSize, 2) * 100];
        
        for(int x = 0; x < chunkSize; x++) {
            for(int z = 0; z < chunkSize; z++) {
                blockIds[calculateCubeIndex(new Vector3(x, (int) (heightMap[chunkX + 50, chunkZ + 50, heightMapPosition] * 10) + standardHeight, z))] = 1;
                blockIds[calculateCubeIndex(new Vector3(x, 0, z))] = 3;
                for(int y = 1; y < standardHeight; y++) {
                    blockIds[calculateCubeIndex(new Vector3(x, y, z))] = 4;
                }
                for(int y = standardHeight; y < (int) (heightMap[chunkX + 50, chunkZ + 50, heightMapPosition] * 10) + standardHeight; y++) {
                    blockIds[calculateCubeIndex(new Vector3(x, y, z))] = 2;
                }
                heightMapPosition++;
            }
        }


        int freeMesh = getFreeMesh();
        Mesh mesh = MeshPooling.meshPrefabs[freeMesh];
        stopwatch.Stop();
        UnityEngine.Debug.Log("It takes: " + stopwatch.ElapsedMilliseconds + "ms!");

        stopwatch = new Stopwatch();
        stopwatch.Start();
        MeshInstance.meshList[freeMesh] = new MeshInstance(mesh, chunkX, chunkZ, blockIds, chunk, freeMesh);
        stopwatch.Stop();
        UnityEngine.Debug.Log("Adding mesh instance takes: " + stopwatch.ElapsedMilliseconds + "ms!");

        chunksLoaded.Add(new Vector2(chunkX, chunkZ));


        
        
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
        triangles[7] = 3 + (cubeNumber * 14);
        triangles[8] = 4 + (cubeNumber * 14);

        triangles[9] = 3 + (cubeNumber * 14);
        triangles[10] = 5 + (cubeNumber * 14);
        triangles[11] = 4 + (cubeNumber * 14);


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

        return x + (z * chunkSize) + (y * (int) Math.Pow(chunkSize, 2));
    }

    public static Vector3 calculateCubeVector(int cubeIndex) {
        float blockX = cubeIndex % chunkSize;
        float blockY = (int) (cubeIndex / (int) Math.Pow(chunkSize, 2));
        float blockZ = (int) ((cubeIndex - (blockY * (int) Math.Pow(chunkSize, 2))) / chunkSize);

        return new Vector3(blockX, blockY, blockZ);
    }

    

    

    public static int getFreeMesh() {
        int i = 0;
        foreach(MeshInstance instance in MeshInstance.meshList) {
            if(instance == null) {
                return i;
            }
            i++;
        }
        return i;
    }




 



}